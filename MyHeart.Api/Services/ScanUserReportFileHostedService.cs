using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyHeart.Data;
using MyHeart.Data.Models;
using MyHeart.DTO.MedicalReport;
using MyHeart.DTO.User;
using MyHeart.Services.Interfaces;
using MyHeart.Services.Interfaces.Azure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MyHeart.Api
{
    public class ScanUserReportFileHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<ScanUserReportFileHostedService> _logger;
        private Timer? _timer = null;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly object lockObject = new object();
        public ScanUserReportFileHostedService(
            ILogger<ScanUserReportFileHostedService> logger,
            IServiceScopeFactory serviceScopeFactory
        )
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");
            using (!ExecutionContext.IsFlowSuppressed() ? (IDisposable)ExecutionContext.SuppressFlow() : null)
            {
                _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(3));
            }
            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            lock (lockObject)
            {
                var scope = _serviceScopeFactory.CreateScope();
                try
                {
                    _logger.LogInformation("Timed Hosted Service is working");
                    string[] allowExtension = { ".pdf", ".jpg", ".jpeg", ".png", "pdf", "jpg", "jpeg", "png" };
                    List<Task> tasks = new List<Task>();

                    var _context = scope.ServiceProvider.GetRequiredService<MyHeartContext>();

                    var userReportFiles = _context.UserReportFile
                        .Include(x => x.UserReport)
                        .Where(f => !f.IsOCRProcessed && allowExtension.Contains(f.Extension))
                        .ToList();

                    foreach (var userReportFile in userReportFiles)
                    {
                        ProcessOCR(userReportFile).Wait();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"ScanUserReportFileHostedService Error{ex}");
                }
                finally
                {
                    scope.Dispose();
                }
            }
            _logger.LogInformation("ScanUserReportFileHostedService Done");
        }

        private async Task ProcessOCR(UserReportFile userReportFile)
        {
            var fileName = $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}_{userReportFile.Name}";
            var stream = new MemoryStream(userReportFile.Content);
            await UploadFileToBlob(stream, fileName, userReportFile.MimeType ?? "application/pdf");
            IFormFile file = new FormFile(
                stream,
                0,
                userReportFile.Content.Length,
                userReportFile.Name,
                fileName
            );

            IServiceScope scope = _serviceScopeFactory.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var _context = serviceProvider.GetRequiredService<MyHeartContext>();
            var _patientMedicalRecordService =
                serviceProvider.GetRequiredService<IPatientMedicalRecordService>();
            var _ocrService = serviceProvider.GetRequiredService<IOCRWebService>();

            var result = await _ocrService.ProcessDocument(file);

            if (result != null)
            {
                string resultUrl = !string.IsNullOrEmpty(result.OutputFileUrl) ?
                    result.OutputFileUrl : !string.IsNullOrEmpty(result.OutputFileUrl2) ?
                    result.OutputFileUrl2 : result.OutputFileUrl3;

                var resultFile = await new HttpClient().GetStringAsync(resultUrl);

                await _patientMedicalRecordService.ScanReportFile(
                    new PatientMedicalRecordDTO
                    {
                        url = fileName,
                        UserReportFileId = userReportFile.Id,
                        UserReport = new UserReportDTO()
                        {
                            Id = userReportFile.UserReport?.Id ?? 0,
                            UserId = userReportFile.UserReport?.UserId ?? 0
                        }
                    },
                    string.Join(Environment.NewLine, result.OCRText.Select(i => string.Join(" ", i))),
                    resultFile ?? string.Join(Environment.NewLine, result.OCRText.Select(i => string.Join(" ", i)))
                );
            }

            userReportFile.IsOCRProcessed = true;
            userReportFile.Content = new byte[0];
            if (userReportFile != null)
                _context.Entry(userReportFile).State = EntityState.Detached;

            _context.Entry(userReportFile).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            scope.Dispose();
        }

        private async Task UploadFileToBlob(Stream stream, string fileName, string contentType)
        {
            IServiceScope scope = _serviceScopeFactory.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var _blobService = serviceProvider.GetRequiredService<IBlob>();

            await _blobService.UploadFileWithContentType(stream, fileName, contentType);
            _logger.LogInformation($"File {fileName} uploaded to blob storage");
            scope.Dispose();
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
