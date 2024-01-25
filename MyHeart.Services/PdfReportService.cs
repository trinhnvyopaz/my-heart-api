using DinkToPdf;
using DinkToPdf.Contracts;
using HandlebarsDotNet;
using Microsoft.EntityFrameworkCore;
using MyHeart.Data;
using MyHeart.Data.Models;
using MyHeart.DTO.User;
using MyHeart.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services
{
    public class PdfReportService : IPdfReportService
    {
        private MyHeartContext _dbContext;
        private IUserService _userService;
        private IAnamnesisService _anamnesisService;
        private IUserNonpharmacyService _userNonpharmacyService;
        private IConverter _pdfConverter;
        private IQuestionService _questionService;

        public PdfReportService(MyHeartContext context, IUserService userService, IAnamnesisService anamnesisService, IUserNonpharmacyService userNonpharmacyService, IConverter pdfCOnverter, IQuestionService questionService)
        {
            _dbContext = context;
            _userService = userService;
            _anamnesisService = anamnesisService;
            _userNonpharmacyService = userNonpharmacyService;
            _pdfConverter = pdfCOnverter;
            _questionService = questionService;
        }

        public async Task<FileDTO> GetChatReportDto(int userId, int questionid)
        {

            var pdfTemplate = await _dbContext.PdfTemplate.FirstOrDefaultAsync(t => t.Code == "Chat");
            if (pdfTemplate == null)
                return null;

            var template = Handlebars.Compile(pdfTemplate.Template);
            var user = await _userService.GetUserDetail(userId);
            if (user == null)
                return null;

            var question = await _questionService.GetQuestionById(questionid, true);
            if (question == null)
                return null;

            var commentRows = question.Comments.Select(c => new CommentRow
            {
                Type = userId == c.SenderId ? "Dotaz" : "Odpověď",
                Text = c.Text
            });

            var now = DateTime.Now;

            var data = new
            {
                Messages = commentRows,
                Subject = question.Subject,
                Name = $"{user.LastName} {user.FirstName}",
                PIN = user.PIN,
                InsuranceNumber = user.InsuranceNumber,
                CurrentDate = $"{now:dd.MM.yyyy hh:mm}",
                ContactAddress = user.Email,
                PhoneContact = user.Phone,
                Title = "Dotaz na zdravotní stav",
                CreatedAt = $"{question.CreationDate:dd.MM.yyyy}"

            };

            var filledHtml = template(data);


            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Grayscale,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4

                },
                Objects = {
                    new ObjectSettings() {
                        HtmlContent = filledHtml,
                        WebSettings = { DefaultEncoding = "utf-8" },
                    }
                }
            };

            var pdfBytes = _pdfConverter.Convert(doc);

            return new FileDTO
            {
                Name = $"UžívanéLéky_{now:ddMMyyyy_hhmmss}",
                Content = pdfBytes,
                Extension = ".pdf",
                MimeType = "application/pdf"
            };
        }

        public async Task<FileDTO> GetPharmaciesReportAsync(int userId)
        {
            var pdfTemplate = await _dbContext.PdfTemplate.FirstOrDefaultAsync(t => t.Code == "Pharmacies");
            if (pdfTemplate == null)
                return null;

            var template = Handlebars.Compile(pdfTemplate.Template);
            var user = await _userService.GetUserDetail(userId);
            if (user == null)
                return null;

            var pharmacies = await _anamnesisService.GetPersonalPharmacy(user.Id);
            var now = DateTime.Now;

            var data = new
            {
                Pharmacies = pharmacies,
                Name = $"{user.LastName} {user.FirstName}",
                PIN = user.PIN,
                InsuranceNumber = user.InsuranceNumber,
                CurrentDate = $"{now:dd.MM.yyyy hh:mm}",
            };

            var filledHtml = template(data);


            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Grayscale,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4

                },
                Objects = {
                    new ObjectSettings() {
                        HtmlContent = filledHtml,
                        WebSettings = { DefaultEncoding = "utf-8" },
                    }
                }
            };

            var pdfBytes = _pdfConverter.Convert(doc);

            return new FileDTO
            {
                Name = $"UžívanéLéky_{now:ddMMyyyy_hhmmss}",
                Content = pdfBytes,
                Extension = ".pdf",
                MimeType = "application/pdf"
            };
        }

        public async Task<FileDTO> GetHealthsStatusReportAsync(int userId)
        {
            var pdfTemplate = await _dbContext.PdfTemplate.FirstOrDefaultAsync(t => t.Code == "HealthStatus");
            if (pdfTemplate == null)
                return null;

            Handlebars.RegisterHelper("formatDate", (writer, context, parameters) =>
            {
                if (context.Value is UserDiseaseDto userDisease)
                {
                    var param = parameters[0];
                    if (param != null)
                    {
                        string format = param.ToString();
                        var date = DateTime.ParseExact(userDisease.StartDateString, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        writer.Write(date.ToString("yyyy"));
                    }
                }
            });

            var template = Handlebars.Compile(pdfTemplate.Template);
            var user = await _userService.GetUserDetail(userId);
            if (user == null)
                return null;

            var now = DateTime.Now;

            var diseaseAnamnesisList = await _anamnesisService.GetDiseaseAnamneses(user.Id);
            var nonpharmacyTherapyList = await _userNonpharmacyService.GetUserNonpharmacies(user.Id);
            var abusus = await _anamnesisService.GetAbususAnamnesis(user.Id);
            var phamacyList = await _anamnesisService.GetPersonalPharmacy(user.Id);
            var recommendations = diseaseAnamnesisList.Select(d => d.Disease.SystemicMeasures);
            var personalAnamnesisList = await _anamnesisService.GetPersonalAnamnesis();
            string height = personalAnamnesisList.FirstOrDefault(p => p.IsPersonal_Type == (int)PersonalAnamnesisType.Height)?.IsPersonal_Value;
            string weight = personalAnamnesisList.FirstOrDefault(p => p.IsPersonal_Type == (int)PersonalAnamnesisType.Weight)?.IsPersonal_Value;
            string bloodPressure = personalAnamnesisList.FirstOrDefault(p => p.IsPersonal_Type == (int)PersonalAnamnesisType.BloodPressure)?.IsPersonal_Value;
            string heartRate = personalAnamnesisList.FirstOrDefault(p => p.IsPersonal_Type == (int)PersonalAnamnesisType.HeartRate)?.IsPersonal_Value;
            string ldl = personalAnamnesisList.FirstOrDefault(p => p.IsPersonal_Type == (int)PersonalAnamnesisType.Cholesterol)?.IsPersonal_Value;

            var abususList = new List<string>
            {
                abusus.IsAbusus_Alcohol ? "pije alkohol" : "nepije alkohol",
                abusus.IsAbusus_Smoker ? "kuřák" : "nekuřák",
                abusus.IsAbusus_Exsmoker ? "v minulosti kouřil" : "nikdy nekouřil"
            };

            var data = new
            {
                Title = "Zdravotní informace",
                Name = $"{user.LastName} {user.FirstName}",
                PIN = user.PIN,
                BirthDate = "",
                ContactAddress = user.Email,
                PhoneContact = user.Phone,
                CurrentDate = $"{now:dd.MM.yyyy hh:mm}",
                Diseases = diseaseAnamnesisList,
                NonpharmaticTherapies = nonpharmacyTherapyList,
                abusus = abususList,
                pharmacies = phamacyList,
                height = height,
                weight = weight,
                bmi = "",
                bloodPressure = bloodPressure,
                heartRate = heartRate,
                ldl = ldl,
                recommendations,
                InsuranceNumber = user.InsuranceNumber
            };

            var filledHtml = template(data);

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Grayscale,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4

                },
                Objects = {
                    new ObjectSettings() {
                        HtmlContent = filledHtml,
                        WebSettings = { DefaultEncoding = "utf-8" },
                    }
                }
            };

            var pdfBytes = _pdfConverter.Convert(doc);

            return new FileDTO
            {
                Name = $"ZdravotniZpráva_{now:ddMMyyyy_hhmmss}",
                Content = pdfBytes,
                Extension = ".pdf",
                MimeType = "application/pdf"
            };
        }
    }

    public class FileDTO
    {
        public string Name { get; set; }
        public string Extension { get; set; }
        public byte[] Content { get; set; }
        public string MimeType { get; set; }
    }

    public class CommentRow
    {
        public string Type { get; set; }
        public string Text { get; set; }
    }
}
