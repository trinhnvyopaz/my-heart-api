using AngleSharp.Common;
using AutoMapper;
using HandlebarsDotNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Logging;
using MoreLinq;
using MyHeart.Data;
using MyHeart.Data.Models;
using MyHeart.DTO;
using MyHeart.DTO.MedicalReport;
using MyHeart.DTO.User;
using MyHeart.Services.Extensions;
using MyHeart.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyHeart.Services
{
    public class PatientMedicalRecordService : IPatientMedicalRecordService
    {
        private readonly MyHeartContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<IPatientMedicalRecordService> _logger;
        private readonly IParameterService _parameterService;
        private IStatisticsService _statisticsService;

        public PatientMedicalRecordService(
            MyHeartContext context,
            IMapper mapper,
            ILogger<PatientMedicalRecordService> logger,
            IParameterService parameterService,
            IStatisticsService statisticsService)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _parameterService = parameterService;
            _statisticsService = statisticsService;
        }

        public async Task<DataTableResponse<PatientMedicalRecordDTO>> Search(
            DataTableRequest request
        )
        {
            try
            {
                var query = _context.PatientMedicalRecords.AsQueryable().AsNoTracking();
                var Page = (request.Page - 1) * request.PageSize;
                if (!string.IsNullOrEmpty(request.Filter))
                {
                    query = query.Where(
                        e =>
                            e.UserReportFile.UserReport.Title.Contains(request.Filter)
                            || e.UserReportFile.Name.Contains(request.Filter)
                    );
                }
                var total = query.Count();

                var dataResult = await query
                    .OrderByDescending(m => m.LastUpdateDate)
                    .Skip(Page)
                    .Take(request.PageSize)
                    .Select(
                        x =>
                            new PatientMedicalRecordDTO()
                            {
                                Id = x.Id,
                                UserReportFileId = x.UserReportFileId,
                                url = x.url,
                                OCRText = x.OCRText,
                                LastUpdateDate = x.LastUpdateDate,
                                UserReportFile = new UserReportFileDTO()
                                {
                                    Name = x.UserReportFile.Name,
                                    Extension = x.UserReportFile.Extension
                                },
                                UserReport = _mapper.Map<UserReportDTO>(x.UserReportFile.UserReport)
                            }
                    )
                    .ToListAsync();

                var result = new DataTableResponse<PatientMedicalRecordDTO>
                {
                    Data = dataResult,
                    TotalCount = total,
                    Page = request.Page,
                    PageSize = request.PageSize
                };
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"MedicalReport Search {ex}");
                throw;
            }
        }

        public async Task<PatientMedicalRecordDTO> Get(int id)
        {
            try
            {
                var data = await _context.PatientMedicalRecords
                    .Include(x => x.UserReportFile)
                    .ThenInclude(x => x.UserReport)
                    .Include(record => record.PatientMedicalExaminations)
                    .FirstOrDefaultAsync(record => record.Id == id);

                if (data == null) return null;

                data.PatientMedicalExaminations = data.PatientMedicalExaminations
                    .OrderBy(x => x.Key)
                    .ToList();

                return new PatientMedicalRecordDTO
                {
                    Id = data.Id,
                    url = data.url,
                    UserReportFileId = data.UserReportFileId,
                    OCRText = data.OCRText,
                    IsDataManager = data.IsDataManager,
                    LastUpdateDate = data.LastUpdateDate,
                    Title = data.UserReportFile?.UserReport?.Title,
                    UserReport = new UserReportDTO()
                    {
                        Id = data.UserReportFile?.UserReport?.Id ?? 0,
                        UserId = data.UserReportFile?.UserReport?.UserId ?? 0
                    },
                    Data = new OptionData
                    {
                        MessageType = _mapper.Map<List<PatientMedicalExaminationDTO>>(
                            data.PatientMedicalExaminations.Where(
                                m => m.Type == MedicalReportType.MessageType
                            )
                        ),
                        MessageDate = _mapper.Map<List<PatientMedicalExaminationDTO>>(
                            data.PatientMedicalExaminations.Where(
                                m => m.Type == MedicalReportType.MessageDate
                            )
                        ),
                        PersonalData = _mapper.Map<List<PatientMedicalExaminationDTO>>(
                            data.PatientMedicalExaminations.Where(
                                m => m.Type == MedicalReportType.PersonalData
                            )
                        ),
                        HealthStatus = _mapper.Map<List<PatientMedicalExaminationDTO>>(
                            data.PatientMedicalExaminations.Where(
                                m => m.Type == MedicalReportType.HealthStatus
                            )
                        ),
                        KnownData = _mapper.Map<List<PatientMedicalExaminationDTO>>(
                            data.PatientMedicalExaminations.Where(
                                m => m.Type == MedicalReportType.KnownData
                            )
                        ),
                        NewlyDiscoveredData = _mapper.Map<List<PatientMedicalExaminationDTO>>(
                            data.PatientMedicalExaminations.Where(
                                m => m.Type == MedicalReportType.NewlyDiscoveredData
                            )
                        ),
                        Other = _mapper.Map<List<PatientMedicalExaminationDTO>>(
                            data.PatientMedicalExaminations.Where(
                                m => m.Type == MedicalReportType.Other
                            )
                        ),
                    }
                };

            }
            catch (Exception ex)
            {
                _logger.LogError($"MedicalReport GetByID {ex}");
                throw;
            }
        }

        public async Task<PatientMedicalRecordDTO> Create(
            PatientMedicalRecordDTO medicalReportDTO,
            string orcText
        )
        {
            try
            {
                var data = _mapper.Map<PatientMedicalRecord>(medicalReportDTO);
                var (listPatient, orcTextHL) = await GetListPatientMedicalExaminations(orcText, medicalReportDTO.UserReport?.UserId ?? 6, orcText);

                data.PatientMedicalExaminations = listPatient;
                data.OCRText = orcTextHL;
                return _mapper.Map<PatientMedicalRecordDTO>(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Create MedicaReport {ex}");
                throw;
            }
        }

        public async Task<PatientMedicalRecordDTO> ScanReportFile(
            PatientMedicalRecordDTO medicalReportDTO,
            string orcText,
            string orcTextURL
        )
        {
            var data = _mapper.Map<PatientMedicalRecord>(medicalReportDTO);

            var (listPatient, orcTextHL) = await GetListPatientMedicalExaminations(orcText, medicalReportDTO.UserReport.UserId, orcTextURL);

            data.PatientMedicalExaminations = listPatient;
            data.OCRText = orcTextHL;

            _context.OCRResults.Add(
                new OCRResult
                {
                    OCRText = orcText,
                    UserReportFilesId = data.UserReportFileId,
                    ProcessedTextContents = data.PatientMedicalExaminations
                        .Select(
                            a =>
                                new ProcessedTextContent
                                {
                                    Key = a.Key,
                                    Value = a.ItemMined,
                                    Type = a.Type,
                                }
                        )
                        .ToList(),
                }
            );

            _context.PatientMedicalRecords.Add(data);
            await _context.SaveChangesAsync();
            return _mapper.Map<PatientMedicalRecordDTO>(data);
        }

        public async Task<PatientMedicalRecordDTO> Update(
            int id,
            PatientMedicalRecordDTO medicalReportDTO
        )
        {
            try
            {
                var patientMedicalRecords = await _context.PatientMedicalRecords
                    .Include(record => record.PatientMedicalExaminations)
                    .FirstOrDefaultAsync(record => record.Id == id);

                if (patientMedicalRecords == null) return null;

                patientMedicalRecords.IsDataManager = true;
                patientMedicalRecords.PatientMedicalExaminations?.Clear();

                var user = await CurrentUserAsync(medicalReportDTO.UserReport?.UserId ?? 0);
                patientMedicalRecords.LastUpdateUser = user.FullName;
                var optionData = medicalReportDTO.Data;

                List<PatientMedicalExaminationDTO> allExaminations = optionData.MessageType
                    .Concat(optionData.MessageDate)
                    .Concat(optionData.PersonalData)
                    .Concat(optionData.HealthStatus)
                    .Concat(optionData.KnownData)
                    .Concat(optionData.NewlyDiscoveredData)
                    .Concat(optionData.Other)
                    .ToList();
                var patientMedicalExaminations = new List<PatientMedicalExamination>();

                foreach (var item in allExaminations)
                {
                    patientMedicalExaminations.Add(
                        new PatientMedicalExamination
                        {
                            Type = item.Type,
                            Key = item.Key,
                            IsSelected = item.IsSelected,
                            ItemMined = item.ItemMined,
                            DateMined = item.DateMined,
                            NoteMined = item.NoteMined,
                            DoseMined = item.DoseMined,
                            DoseUnitMined = item.DoseUnitMined,
                            FrequencyMined = item.FrequencyMined,
                            DiseasePharmacyId = item.DiseasePharmacyId,
                            BeforeMarkerReally = item.BeforeMarkerReally,
                            KeywordRelly = item.KeywordRelly,
                            DoseMinedRelly = item.DoseMinedRelly,
                            DateMinedArea = item.DateMinedArea,
                        }
                    );
                }

                var listUserAnamnesis = await _context.UserAnamnesis
                    .Where(x => x.UserId == user.Id)
                    .Where(x => x.IsPersonalAnamnesis)
                    .ToListAsync();

                //Health Status
                var listNewAnamnesis = GetListUserAnamnesis(
                    user.Id,
                    user.FullName,
                    listUserAnamnesis,
                    optionData.HealthStatus
                );

                //Newly Detected Data
                var (listDiseases, listPharmacy, listNonpharmacy) = await GetListDiseasesNonpharmacy(
                    medicalReportDTO,
                    optionData.NewlyDiscoveredData,
                    optionData.Other
                );

                var dbuserPharmacies = await _context.UserPharmacy
                    .Where(p => p.UserId == user.Id)
                    .ToListAsync();
                List<UserPharmacy> listRemovePharmacy = new List<UserPharmacy>();

                listPharmacy = listPharmacy.DistinctBy(x=>x.Name).ToList();
                foreach (var pharma in listPharmacy)
                {
                    var modelToRemove = dbuserPharmacies.FirstOrDefault(x => x.Name == pharma.Name);
                    if (modelToRemove != null)
                    {
                        listRemovePharmacy.Add(modelToRemove);
                    }
                }
                _context.RemoveRange(listRemovePharmacy);
                _context.UserPharmacy.AddRange(listPharmacy);
                _context.UserAnamnesis.UpdateRange(listNewAnamnesis);
                _context.UserDiseases.UpdateRange(listDiseases);
                _context.UserNonpharmacy.UpdateRange(listNonpharmacy);

                patientMedicalRecords.PatientMedicalExaminations = patientMedicalExaminations;
                await _context.SaveChangesAsync();

                return _mapper.Map<PatientMedicalRecordDTO>(patientMedicalRecords);
            }
            catch (Exception ex)
            {
                _logger.LogError($"MedicalReport Update {ex}");
                throw;
            }
        }

        public async Task<PatientMedicalRecordDTO> Delete(int id)
        {
            var data = await _context.PatientMedicalRecords.FindAsync(id);
            _context.PatientMedicalRecords.Remove(data);
            _logger.LogInformation("MedicalReportService.Delete: {0} successfully", data.Id);
            await _context.SaveChangesAsync();
            return _mapper.Map<PatientMedicalRecordDTO>(data);
        }
        public async Task<FileDTO> ExportExcel(int id)
        {
            try
            {
                var patientMedicalRecords = await _context.PatientMedicalRecords
                    .Include(x => x.UserReportFile)
                    .ThenInclude(x => x.UserReport)
                    .Include(record => record.PatientMedicalExaminations)
                    .FirstOrDefaultAsync(record => record.Id == id);

                var listParameter = await _context.Parameter
                   .Include(x => x.Markers).ToListAsync();

                List<MedicalReportExport> listExport = new List<MedicalReportExport>();
                string ocrText = Regex.Replace(patientMedicalRecords.OCRText, @"<[^>]*(style\s*=\s*""[^""]*"")[^>]*>|<[^>]*[^>]*>", string.Empty);

                foreach (var item in patientMedicalRecords.PatientMedicalExaminations.OrderBy(x => x.Key))
                {
                    var parameter = listParameter.FirstOrDefault(x => x.Key == item.Key && x.Type == item.Type);
                    string beforeMarker = string.Empty;
                    var medicalReport = new MedicalReportExport();

                    if (parameter?.Markers != null)
                    {
                        beforeMarker = string.Join(",", parameter?.Markers?.Where(x => x.MakerGroup == MakerGroup.Before).Select(c => c.Value));
                    }

                    medicalReport.DataSet = GetEnumDescription(item.Type);
                    medicalReport.DataClass = GetEnumDescription(item.Key);
                    medicalReport.ItemMined = item.KeywordRelly;
                    medicalReport.DateMined = item.DateMined;
                    medicalReport.NoteMined = item.NoteMined;
                    medicalReport.DoseMined = item.DoseMined;
                    medicalReport.DoseUnitMined = item.DoseUnitMined; 
                    medicalReport.FrequencyMined = item.DoseMinedRelly;
                    medicalReport.BeforeMarker = beforeMarker;
                    medicalReport.BeforeMarkerReally = item.BeforeMarkerReally;
                    medicalReport.SearchArea = parameter == null ? string.Empty : GetEnumDescription((SearchAreaType)parameter.SearchType);
                    medicalReport.KeywordSource = parameter?.Keywords;
                    medicalReport.KeywordRelly = (item.Key == MedicalReportKey.MessageDate || item.Key == MedicalReportKey.Workplace) ?
                        string.Empty : item.ItemMined; // Keyword Really used
                    medicalReport.ResultFormat = parameter?.ResultFormat;
                    medicalReport.ResultCondition = parameter?.ResultCondition;
                    medicalReport.Match = parameter == null ? string.Empty : GetEnumDescription(parameter.ConformityType);
                    medicalReport.ResultExclusion = parameter?.ResultExclusion;

                    medicalReport.ResultExclusionReally = item.Key >= MedicalReportKey.KnownIllness && !string.IsNullOrEmpty(parameter?.ResultExclusion) ?
                        "0" : item.Type == MedicalReportType.MessageType ?
                        "1" : string.Empty;

                    medicalReport.DateMinedArea = item.DateMinedArea;
                    medicalReport.DoseMinedSource = (item.Key == MedicalReportKey.NewPharmacy ||
                        item.Key == MedicalReportKey.AWellKnownPharmacy) ? "Značka dávkování" : string.Empty;
                    medicalReport.DoseMinedRelly = item.FrequencyMined; // Dosage for Mined Pharmacy - Really used

                    if (!string.IsNullOrEmpty(item.ItemMined) && !string.IsNullOrEmpty(item.DateMined))
                    {
                        Match matchText = new Regex(string.Format(@"(?<={0})(.*)({1})", item.ItemMined, item.DateMined), RegexOptions.IgnoreCase).Match(ocrText);
                        medicalReport.DateMinedReal = Math.Abs(matchText.Value.IndexOf(item.DateMined)).ToString();
                    }

                    if (!string.IsNullOrEmpty(item.BeforeMarkerReally))
                    {
                        Match matchText = new Regex(string.Format(@"(?<={0})(.*)({1})", item.BeforeMarkerReally, item.ItemMined), RegexOptions.IgnoreCase).Match(ocrText);
                        medicalReport.SearchAreaReal = Math.Abs(matchText.Value.IndexOf(item.ItemMined)).ToString();
                    }

                    listExport.Add(medicalReport);
                }

                string exportPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Templates", "MiningAlgorithmAnalytical.xlsx");
                FileInfo file = new FileInfo(exportPath);
                byte[] fileContent = _statisticsService.ExportToExcelNew(listExport, file);

                return new FileDTO
                {
                    Name = $"MiningAlgorithmAnalytical_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}",
                    Content = fileContent,
                    Extension = ".xlsx",
                    MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"MedicalReportExport {ex}");
                throw;
            }
        }
        #region Private Methods
        private async Task<(List<PatientMedicalExamination>, string)> GetListPatientMedicalExaminations(string orcText, int userId, string orcTextURL)
        {
            var listExtractDataOCR = await _parameterService.ExtractDataOCR(orcText);
            var listPatientMedical = new List<PatientMedicalExamination>();
            string textHighlight = string.Empty;

            if (listExtractDataOCR != null)
            {
                HandleMessageType(listExtractDataOCR, listPatientMedical);

                HandleMessageDate(listExtractDataOCR, listPatientMedical);

                await HandlePersonalData(listExtractDataOCR, listPatientMedical, userId);

                HandleHealthStatus(listExtractDataOCR, listPatientMedical);

                var listUserDiseases = await _context.UserDiseases
                    .Where(x => x.UserId == userId)
                    .Select(x => new Disease() { Name = x.Disease.Name, Id = x.DiseaseId })
                    .AsNoTracking()
                    .ToListAsync();

                var listDiseases = await _context.Disease
                    .Select(x => new Disease() { Name = x.Name, Id = x.Id })
                    .AsNoTracking()
                    .ToListAsync();

                var listNonpharmaticTherapy = await _context.NonpharmaticTherapy
                    .Select(x => new Disease() { Name = x.Name, Id = x.Id })
                    .AsNoTracking()
                    .ToListAsync();

                var listUserNonpharmacy = await _context.UserNonpharmacy
                    .Where(x => x.UserId == userId)
                    .Select(
                        x =>
                            new Disease()
                            {
                                Name = x.NonpharmaticTherapy.Name,
                                Id = x.NonpharmaticTherapyId
                            }
                    )
                    .AsNoTracking()
                    .ToListAsync();

                HandleDiseaseNonpharmacy(
                    listExtractDataOCR,
                    listPatientMedical,
                    listUserDiseases,
                    listDiseases,
                    listUserNonpharmacy,
                    listNonpharmaticTherapy
                );

                // Known Treatment & New Treatment
                var listUserPharmacy = await _context.UserPharmacy
                    .Where(x => x.UserId == userId)
                    .Select(x => new Pharmacy() { Id = x.PharmacyId ?? 0, Name = x.Pharmacy.Name, Strength = x.Pharmacy.Strength })
                    .AsNoTracking()
                    .ToListAsync();

                var listPharmacy = await _context.Pharmacy
                    .Select(x => new Pharmacy() { Id = x.Id, Name = x.Name, Strength = x.Strength })
                    .AsNoTracking()
                    .ToListAsync();

                HandleKnownTreatment(
                    listExtractDataOCR,
                    listPatientMedical,
                    listUserPharmacy,
                    listPharmacy
                );

                HandleDiscontinuedTreatment(listExtractDataOCR, listPatientMedical, listPharmacy);

                textHighlight = HighlightKeywords(orcTextURL, listExtractDataOCR, listPatientMedical);
            }
            return (listPatientMedical, textHighlight);
        }
        private string HighlightKeywords(string orcText,
            List<ParameterDTO> listParameterDTO,
            List<PatientMedicalExamination> listPatientMedical)
        {

            //string[] lines = orcText.Split('\n');
            //List<string> listText = new List<string>();

            //foreach (var item in lines)
            //{
            //    if(item.Split(" ").Length > 1)  listText.Add($"{item.Split(" ")[0]} {item.Split(" ")[1]}");
            //}

            //orcText = Regex.Replace(orcText, @"\t|\n|\r", "");

            if (string.IsNullOrEmpty(orcText)) return orcText;

            foreach (var item in listParameterDTO.Where(x => x.Type != MedicalReportType.KnownData))
            {
                if (string.IsNullOrEmpty(item.Value) || item.Type == MedicalReportType.MessageType)
                    continue;

                var regex = new Regex(Regex.Escape(item.Value), RegexOptions.IgnoreCase);
                orcText = regex.Replace(orcText, match => $"<span style=\"background-color: {GetColor(item.Type)};\">{match.Value}</span>");

            }

            var patientMedicals = listPatientMedical.Where(x => x.Key == MedicalReportKey.KnownIllness ||
                x.Key == MedicalReportKey.KnownPerformances ||
                x.Key == MedicalReportKey.NewDiseases ||
                x.Key == MedicalReportKey.NewPerformances).ToList();

            foreach (var data in patientMedicals)
            {
                if (!string.IsNullOrEmpty(data.NoteMined))
                {
                    var regexNote = new Regex(Regex.Escape($"{data.ItemMined}{data.NoteMined}"), RegexOptions.IgnoreCase);
                    var regexItemMined = new Regex(Regex.Escape(data.ItemMined), RegexOptions.IgnoreCase);
                    var regexDate = new Regex($@"({data.ItemMined})(.*)({data.DateMined})", RegexOptions.IgnoreCase);

                    if (regexNote.IsMatch(orcText))
                    {
                        orcText = regexNote.Replace(orcText, match => $"<span style=\"background-color:" +
                        $"{(data.Key == MedicalReportKey.KnownIllness || data.Key == MedicalReportKey.KnownPerformances ? "#FDFD96" : "#FFAF42")}" +
                        $";\">{match.Value}</span>");
                    }
                    else if (regexDate.IsMatch(orcText))
                    {
                        orcText = regexDate.Replace(orcText, match => $"<span style=\"background-color: {GetColor(data.Type)};\">{match.Value}</span>");
                    }
                    else
                    {
                        orcText = regexItemMined.Replace(orcText, match => $"<span style=\"background-color:" +
                        $"{(data.Key == MedicalReportKey.KnownIllness || data.Key == MedicalReportKey.KnownPerformances ? "#FDFD96" : "#FFAF42")}" +
                        $";\">{match.Value}</span>");
                    }
                }

                //if (!string.IsNullOrEmpty(data.ItemMined))
                //{
                //    var regex = new Regex(Regex.Escape(data.ItemMined), RegexOptions.IgnoreCase);

                //    orcText = regex.Replace(orcText, match => $"<span style=\"background-color:" +
                //        $"{(data.Key == MedicalReportKey.KnownIllness || data.Key == MedicalReportKey.KnownPerformances ? "#FDFD96" : "#FFAF42")}" +
                //        $";\">{match.Value}</span>");
                //}

                //if (!string.IsNullOrEmpty(data.DateMined))
                //{
                //    var regexDate = new Regex(Regex.Escape(data.DateMined), RegexOptions.IgnoreCase);
                //    orcText = regexDate.Replace(orcText, match => $"<span style=\"background-color:" +
                //        $"{(data.Key == MedicalReportKey.KnownIllness || data.Key == MedicalReportKey.KnownPerformances ? "#FDFD96" : "#FFAF42")}" +
                //        $";\">{match.Value}</span>");
                //}
            }


            var listAWellKnownPharmacy = listPatientMedical.Where(x => x.Key == MedicalReportKey.AWellKnownPharmacy ||
                x.Key == MedicalReportKey.NewPharmacy).ToList();

            foreach (var data in listAWellKnownPharmacy)
            {
                if (!string.IsNullOrEmpty(data.NoteMined))
                {
                    var regex = new Regex(Regex.Escape(data.NoteMined), RegexOptions.IgnoreCase);
                    string itemMuptiple = $@"({data.ItemMined.Replace(")", string.Empty)})(.*)({data.FrequencyMined})";
                    var regexItemMutiple = new Regex(itemMuptiple, RegexOptions.IgnoreCase);
                    var regexItemMined = new Regex(Regex.Escape(data.ItemMined), RegexOptions.IgnoreCase);

                    if (regex.IsMatch(orcText))
                    {
                        orcText = regex.Replace(orcText, match => $"<span style=\"background-color: {GetColor(data.Type)};\">{match.Value}</span>");
                    }
                    else if (regexItemMutiple.IsMatch(orcText))
                    {
                        orcText = regexItemMutiple.Replace(orcText, match => $"<span style=\"background-color: {GetColor(data.Type)};\">{match.Value}</span>");
                    }
                    else
                    {
                        orcText = regexItemMined.Replace(orcText, match => $"<span style=\"background-color: {GetColor(data.Type)};\">{match.Value}</span>");
                    }
                }
                data.NoteMined = string.Empty;
            }

            //foreach( var data in listText)
            //{
            //    if(!string.IsNullOrEmpty(data))  orcText = orcText.Replace(data, $"<br>{data}");
            //}
            return orcText;
        }
        private string GetColor(MedicalReportType input)
        {
            switch (input)
            {
                case MedicalReportType.MessageDate:
                    return "#FFC0CB";
                case MedicalReportType.PersonalData:
                    return "#A5D6A7";
                case MedicalReportType.HealthStatus:
                    return "#00FFFF";
                case MedicalReportType.KnownData:
                    return "#FFFF8D";
                case MedicalReportType.NewlyDiscoveredData:
                    return "#FFA726";
                default:
                    return "#009688";
            }
        }
        private void HandleMessageType(
            List<ParameterDTO> listExtractDataOCR,
            List<PatientMedicalExamination> listPatientMedical
        )
        {
            var extractDataOCR = listExtractDataOCR.FirstOrDefault(x => x.Type == MedicalReportType.MessageType);

            if (extractDataOCR != null)
            {
                listPatientMedical.Add(
                    new PatientMedicalExamination
                    {
                        Type = MedicalReportType.MessageType,
                        Key = MedicalReportKey.MessageType,
                        ItemMined = extractDataOCR?.Value == "1" ? "Ambulantní zpráva" : "Hospitalizace",
                        IsSelected = true,
                    }
                );
            }
        }

        private void HandleMessageDate(
            List<ParameterDTO> listExtractDataOCR,
            List<PatientMedicalExamination> listPatientMedical
        )
        {
            var extractDataOCR = listExtractDataOCR.FirstOrDefault(
                x => x.Type == MedicalReportType.MessageDate
            );
            //DateTime dateTime;

            //if (extractDataOCR != null
            //    && !string.IsNullOrEmpty(extractDataOCR.Value)
            //    && DateTime.TryParse(extractDataOCR.Value, out dateTime))
            //{
            //    isSelected = true;
            //    extractDataOCR.Value = dateTime.ToString("yyyy-MM-dd");
            //}

            if (extractDataOCR != null)
            {
                listPatientMedical.Add(
                    new PatientMedicalExamination
                    {
                        Type = MedicalReportType.MessageDate,
                        Key = MedicalReportKey.MessageDate,
                        ItemMined = extractDataOCR?.Value,
                        BeforeMarkerReally = extractDataOCR?.BeforeMarkerReally,
                        IsSelected = true,
                    }
                );
            }
        }

        private async Task HandlePersonalData(
            List<ParameterDTO> listExtractDataOCR,
            List<PatientMedicalExamination> listPatientMedical,
            int userId
        )
        {
            var listPersonalData = listExtractDataOCR.Where(x => x.Type == MedicalReportType.PersonalData).ToList();
            string descriptionName = MedicalReportConsts.PersonalNameOne;
            int matchName = 1;

            var user = await CurrentUserAsync(userId);
            var userDetail = await _context.UserDetail.FirstOrDefaultAsync(x => x.UserId == userId);

            foreach (var item in listPersonalData)
            {
                if (string.IsNullOrEmpty(item.Value)) continue;

                if (item.Key == MedicalReportKey.Workplace)
                {

                    var listMedicalFacilities = await _context.MedicalFacilities
                        .Select(x => new MedicalFacilities() { Name = x.Name })
                        .AsNoTracking()
                        .ToListAsync();

                    item.Value = listMedicalFacilities.FirstOrDefault(x => item.Value.ToLower().Contains(x.Name.ToLower()))?.Name;
                }
                else if (item.Key == MedicalReportKey.Name || item.Key == MedicalReportKey.Surname)
                {
                    RegexHealthCondition(item, @"(.+?)(=?Datum|Rodné)");

                    string fullName = item.Value.TrimStart(' ', ':');
                    string[] namePerson = fullName.Split(' ');

                    if (item.Key == MedicalReportKey.Name)
                    {
                        if (user != null
                            && fullName.ToLower().Contains(user.FirstName?.ToLower())
                            && fullName.ToLower().Contains(user.LastName?.ToLower()))
                        {
                            item.Value = user.FirstName;
                            matchName = 3;
                            descriptionName = MedicalReportConsts.PersonalNameThree;
                        }
                        else
                        {
                            if (_context.Users.Any(x => fullName.ToLower().Contains(x.FirstName.ToLower()) &&
                                fullName.ToLower().Contains(x.LastName.ToLower())))
                            {
                                matchName = 2;
                                descriptionName = MedicalReportConsts.PersonalNameTow;
                            }
                            item.Value = string.Empty;
                        }
                    }

                    if (item.Key == MedicalReportKey.Surname && namePerson.Length > 1)
                    {
                        if (user != null
                            && fullName.ToLower().Contains(user.FirstName?.ToLower())
                            && fullName.ToLower().Contains(user.LastName?.ToLower()))
                        {
                            item.Value = user.LastName;
                        }
                        else item.Value = string.Empty;
                    }
                }
                else if (item.Key == MedicalReportKey.PersonalIdentificationNumber)
                {
                    if (userDetail != null && item.Value.ToLower().Contains(userDetail?.PIN?.ToLower()))
                    {
                        item.Value = userDetail?.PIN;
                    }
                    else
                    {
                        //item.Value = RegexDataByPattern(item.Value, @"\d+\s[a-zA-Z]");
                        item.Value = string.Empty;
                    }
                }

                if (item.Key == MedicalReportKey.Address)
                {
                    string userAddress = $"{userDetail?.Street} {userDetail?.StreetNumber} , {userDetail?.City}, {userDetail?.Zip}";
                    if (item.Value.ToLower().Contains(userAddress.ToLower()))
                    {
                        item.Value = userAddress;
                    }
                    else
                    {
                        //string pattern = @"(.*?)(\d+\s+\d+)";
                        //Match matche = Regex.Match(item.Value.TrimStart(':'), pattern);
                        //item.Value = matche.Value;

                        item.Value = string.Empty;
                    }
                }

                listPatientMedical.Add(
                    new PatientMedicalExamination
                    {
                        Type = MedicalReportType.PersonalData,
                        Key = item.Key,
                        ItemMined = !string.IsNullOrEmpty(item.Value) ? item.Value.TrimStart(' ', ':') : string.Empty,
                        IsSelected = !string.IsNullOrEmpty(item.Value),
                        NoteMined = (item.Key == MedicalReportKey.Name) ? descriptionName : string.Empty,
                        DiseasePharmacyId = (item.Key == MedicalReportKey.Name) ? matchName : 0,
                        BeforeMarkerReally = item.BeforeMarkerReally,
                        KeywordRelly = (item.Key != MedicalReportKey.Workplace && !string.IsNullOrEmpty(item.Value)) ?
                            item.Value.TrimStart(' ', ':') : string.Empty,
                    }
                );
            }
        }

        private void HandleHealthStatus(
            List<ParameterDTO> listExtractDataOCR,
            List<PatientMedicalExamination> listPatientMedical
        )
        {
            var listHealthCondition = listExtractDataOCR
                .Where(x => x.Type == MedicalReportType.HealthStatus)
                .ToList();

            foreach (var item in listHealthCondition)
            {
                if (!string.IsNullOrEmpty(item?.Value))
                {
                    if (item.Key == MedicalReportKey.Height)
                        RegexHealthCondition(item, @"(\d+|\d+,\d+)\s*cm\b");

                    if (item.Key == MedicalReportKey.Weight)
                        RegexHealthCondition(item, @"(\d+|\d+,\d+)\s*(kg\b)");

                    if (item.Key == MedicalReportKey.BloodPressure)
                        RegexHealthCondition(item, @"(\d+/\d+)|(\d+/\d+)\s*mmHg\b");

                    if (item.Key == MedicalReportKey.Pulse)
                        RegexHealthCondition(item, @"(\d+)/min\b");

                    if (item.Key == MedicalReportKey.LDL)
                        RegexHealthCondition(item, @"(\d+/\d+|\d+/\d+)\s*(mg/dL\b|mmol/l\b)");

                    listPatientMedical.Add(
                        new PatientMedicalExamination
                        {
                            Type = MedicalReportType.HealthStatus,
                            Key = item.Key,
                            ItemMined = item.Value,
                            BeforeMarkerReally = item.BeforeMarkerReally,
                            IsSelected = true,
                            KeywordRelly = item.Value,
                        }
                    );
                }
            }
        }

        private void HandleDiseaseNonpharmacy(
            List<ParameterDTO> listExtractDataOCR,
            List<PatientMedicalExamination> listPatientMedical,
            List<Disease> listUserDiseases,
            List<Disease> listDisease,
            List<Disease> listUserNonpharmacy,
            List<Disease> listNonpharmaticTherapy
        )
        {
            var knownIllness = listExtractDataOCR.FirstOrDefault(
                x => x.Type == MedicalReportType.KnownData && x.Key == MedicalReportKey.KnownIllness
            );

            if (!string.IsNullOrEmpty(knownIllness?.Value))
            {
                listDisease = listDisease
                    .Where(x => knownIllness.Value.ToLower().Contains(x.Name.ToLower()))
                    .ToList();
                listNonpharmaticTherapy = listNonpharmaticTherapy
                    .Where(x => knownIllness.Value.ToLower().Contains(x.Name.ToLower()))
                    .ToList();

                var listDiseaseNonpharmacy = listDisease.Select(x => x.Name).ToList();
                listDiseaseNonpharmacy.AddRange(listNonpharmaticTherapy.Select(x => x.Name));

                string[] arrDiseaseNonpharmacy = knownIllness.Value.Split(
                    listDiseaseNonpharmacy.ToArray(),
                    StringSplitOptions.None
                );

                foreach (var item in listDisease)
                {
                    ProcessDiseaseNonpharmacy(
                        listPatientMedical,
                        listUserDiseases,
                        knownIllness,
                        listDiseaseNonpharmacy,
                        arrDiseaseNonpharmacy,
                        item,
                        true
                    );
                }

                foreach (var item in listNonpharmaticTherapy)
                {
                    ProcessDiseaseNonpharmacy(
                        listPatientMedical,
                        listUserNonpharmacy,
                        knownIllness,
                        listDiseaseNonpharmacy,
                        arrDiseaseNonpharmacy,
                        item,
                        false
                    );
                }
            }
        }

        private void ProcessDiseaseNonpharmacy(
            List<PatientMedicalExamination> listPatientMedical,
            List<Disease> listUserDiseases,
            ParameterDTO knownIllness,
            List<string> listDiseaseNonpharmacy,
            string[] arrDiseaseNonpharmacy,
            Disease disease,
            bool isDisease
        )
        {
            string patternDate = @"(\d{4}\b|\d{1,2}\.\d{1,2}\.\d{4}\b|\d{1,2}/\d{4}\b)";
            string patternNote = @"(?<=\s{0}\s)(.*)";
            var userDiseases = listUserDiseases.FirstOrDefault(x => x.Name.ToLower() == disease.Name.ToLower());
            string  note = string.Empty;

            foreach (var item in arrDiseaseNonpharmacy)
            {
                var notes = $"{userDiseases?.Name ?? disease.Name}{item}";
                Match matchNote = new Regex($@"(\s{userDiseases?.Name ?? disease.Name}\s)", RegexOptions.IgnoreCase).Match(knownIllness.Value);
                if (matchNote.Success && knownIllness.Value.Contains(notes))
                    note = item;
            }

            if (string.IsNullOrEmpty(note))
            {
                var patternNotes = string.Concat(
                patternNote.Replace("{0}", userDiseases?.Name ?? disease.Name),
                $"(?={string.Join("|", listDiseaseNonpharmacy)})"
                );
                var matchDescription = new Regex(patternNotes).Match(knownIllness.Value);
                note = matchDescription.Success ? matchDescription.Value : string.Empty;
            }

            if (string.IsNullOrEmpty(note))
            {
               var matchDescription = new Regex(patternNote.Replace("{0}", userDiseases?.Name ?? disease.Name)).Match(knownIllness.Value);
               note = matchDescription.Success ? matchDescription.Value : string.Empty;
            }

            if (string.IsNullOrEmpty(note)) return;

            //patternDate = patternDate.Replace("{0}", userDiseases?.Name ?? disease.Name);
            Match matchKnownIllness = new Regex(patternDate, RegexOptions.IgnoreCase).Match(note);
            string diseaseDate = matchKnownIllness.Success? matchKnownIllness.Groups[1].Value : string.Empty;
            string dateMinedArea = string.Empty;

            if (!string.IsNullOrEmpty(diseaseDate))
                dateMinedArea = Math.Abs(note.IndexOf(diseaseDate)).ToString();
            
            if (userDiseases != null)
            {
                listPatientMedical.Add(
                    new PatientMedicalExamination
                    {
                        Type = MedicalReportType.KnownData,
                        Key = isDisease ? MedicalReportKey.KnownIllness : MedicalReportKey.KnownPerformances,
                        ItemMined = userDiseases.Name,
                        DateMined = diseaseDate,
                        NoteMined = note,
                        DiseasePharmacyId = userDiseases.Id,
                        KeywordRelly = userDiseases.Name,
                        DateMinedArea = dateMinedArea,
                        IsSelected = true,
                    }
                );
            }
            else
            {
                listPatientMedical.Add(
                    new PatientMedicalExamination
                    {
                        Type = MedicalReportType.NewlyDiscoveredData,
                        Key = isDisease ? MedicalReportKey.NewDiseases : MedicalReportKey.NewPerformances,
                        ItemMined = disease.Name,
                        DateMined = diseaseDate,
                        NoteMined = note,
                        DiseasePharmacyId = disease.Id,
                        KeywordRelly = disease.Name,
                        DateMinedArea = dateMinedArea,
                        IsSelected = true,
                    }
                );
            }
        }

        private void HandleKnownTreatment(
            List<ParameterDTO> listExtractDataOCR,
            List<PatientMedicalExamination> listPatientMedical,
            List<Pharmacy> listUserPharmacy,
            List<Pharmacy> listPharmacy
        )
        {
            var knownPharmacy = listExtractDataOCR.FirstOrDefault(
                x =>
                    x.Type == MedicalReportType.KnownData
                    && x.Key == MedicalReportKey.AWellKnownPharmacy
            );
            if (!string.IsNullOrEmpty(knownPharmacy?.Value))
            {
                string[] resultDosage = knownPharmacy.Value.Split(
                    MedicalReportConsts.PharmacyDose.Split("|"),
                    StringSplitOptions.RemoveEmptyEntries
                );
                resultDosage = resultDosage.Distinct().ToArray();

                foreach (string dosage in resultDosage)
                {
                    string patternPharmacy = @"({0})(.*?)(keyDosage)".Replace("keyDosage", MedicalReportConsts.PharmacyDose);

                    string patternDose = @"(\d+/\d+,\d+/\d*|\d+,\d+|\d+.\d+\s*|\d+)";
                    string patternItem = @"({0})(.+?)(?=\)|\(|\d+|mg|Oug)";

                    var userPharmacy = listUserPharmacy.FirstOrDefault(
                        x => dosage.ToLower().Contains(x.Name.ToLower())
                    );
                    var pharmacy = listPharmacy.FirstOrDefault(
                        x => dosage.ToLower().Contains(x.Name.ToLower())
                    );
                    var diseasePharmacyId = userPharmacy?.Id ?? pharmacy?.Id;

                    if (listPatientMedical.Any(x => x.DiseasePharmacyId == diseasePharmacyId))
                        continue;

                    Match matchPharmacy = new Regex(
                        string.Format(patternPharmacy, userPharmacy?.Name ?? pharmacy?.Name),
                        RegexOptions.IgnoreCase
                    ).Match(knownPharmacy.Value);

                    string note = matchPharmacy.Success ? matchPharmacy.Value : string.Empty;
                    string itemMined = RegexDataByPattern(matchPharmacy.Value, string.Format(patternItem, userPharmacy?.Name ?? pharmacy?.Name));
                    string doseMined = "";
                    string doseUnitMined = "";
                    string frequencyMined = matchPharmacy.Groups.Count > 2 ? matchPharmacy.Groups[3].Value : string.Empty;

                    if (matchPharmacy.Groups.Count > 2)
                    {
                        MatchCollection matchDose = new Regex(patternDose + @"\s*(mg|ml|g|mg/ml|iu/ml|MG/DÁV|Oug)", RegexOptions.IgnoreCase)
                            .Matches(matchPharmacy.Groups[2].Value);

                        if (matchDose.Count > 0)
                        {
                            foreach (Match match in matchDose)
                            {
                                GroupCollection groups = match.Groups;
                                doseMined += !string.IsNullOrEmpty(groups[1].Value) ? groups[1].Value + "/" : string.Empty;
                                doseUnitMined += !string.IsNullOrEmpty(groups[2].Value) ? groups[2].Value + "/" : string.Empty;
                            }
                            doseMined = doseMined.TrimEnd('/');
                            doseUnitMined = doseUnitMined.TrimEnd('/');
                        }
                        else
                        {
                            doseMined = RegexDataByPattern(matchPharmacy.Groups[2].Value, patternDose);
                        }
                    }

                    // Known treatment
                    if (userPharmacy != null && !string.IsNullOrEmpty(itemMined))
                    {
                        listPatientMedical.Add(
                            new PatientMedicalExamination
                            {
                                Type = MedicalReportType.KnownData,
                                Key = MedicalReportKey.AWellKnownPharmacy,
                                ItemMined = itemMined,
                                DoseMined = doseMined,
                                DoseUnitMined = doseUnitMined,
                                FrequencyMined = frequencyMined,
                                DoseMinedRelly = frequencyMined,
                                KeywordRelly = itemMined,
                                IsSelected = true,
                                NoteMined = note
                            }
                        );
                        continue;
                    }

                    // A new treatment
                    if (pharmacy != null && !string.IsNullOrEmpty(itemMined))
                    {
                        listPatientMedical.Add(
                            new PatientMedicalExamination
                            {
                                Type = MedicalReportType.NewlyDiscoveredData,
                                Key = MedicalReportKey.NewPharmacy,
                                ItemMined = itemMined,
                                DoseMined = doseMined,
                                DoseUnitMined = doseUnitMined,
                                FrequencyMined = frequencyMined,
                                DoseMinedRelly = frequencyMined,
                                DiseasePharmacyId = pharmacy.Id,
                                KeywordRelly = itemMined,
                                IsSelected = true,
                                NoteMined = note
                            }
                        );
                    }
                }
            }
        }

        private void HandleDiscontinuedTreatment(
            List<ParameterDTO> listExtract,
            List<PatientMedicalExamination> listPatientMedical,
            List<Pharmacy> listPharmacy
        )
        {
            var discontinuedTreatment = listExtract.FirstOrDefault(
                x =>
                    x.Type == MedicalReportType.NewlyDiscoveredData
                    && x.Key == MedicalReportKey.DiscontinuedTreatment
            );

            if (!string.IsNullOrEmpty(discontinuedTreatment?.Value))
            {
                listPharmacy = listPharmacy
                    .Where(x => discontinuedTreatment.Value.ToLower().Contains(x.Name.ToLower()))
                    .ToList();

                foreach (var item in listPharmacy)
                {
                    listPatientMedical.Add(
                        new PatientMedicalExamination
                        {
                            Type = MedicalReportType.NewlyDiscoveredData,
                            Key = MedicalReportKey.DiscontinuedTreatment,
                            ItemMined = item.Name,
                            IsSelected = true,
                        }
                    );
                }
            }
        }

        private void RegexHealthCondition(ParameterDTO item, string pattern)
        {
            if (!string.IsNullOrEmpty(item.Value))
            {
                Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

                Match match = regex.Match(item.Value);

                item.Value = match.Success ? match.Groups[1].Value : item.Value;
            }
        }

        private string RegexDataByPattern(string itemValue, string pattern)
        {
            if (string.IsNullOrEmpty(itemValue))
                return string.Empty;

            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            Match match = regex.Match(itemValue);

            return match.Success ? match.ToString() : string.Empty;
        }

        private async Task<(List<UserDisease>, List<UserPharmacy>, List<UserNonpharmacy>)> GetListDiseasesNonpharmacy(
            PatientMedicalRecordDTO medicalReportDTO,
            List<PatientMedicalExaminationDTO> newlyDiscoveredData,
            List<PatientMedicalExaminationDTO> optionDataOther)
        {
            var listUserDiseases = new List<UserDisease>();
            var listUserPharmacy = new List<UserPharmacy>();
            var listUserNonpharmacy = new List<UserNonpharmacy>();
            var listNewlyDiscoveredData = newlyDiscoveredData.Concat(optionDataOther).ToList();

            var listNewDiseases = listNewlyDiscoveredData
                .Where(x => x.Key == MedicalReportKey.NewDiseases)
                .ToList();
            var listNewPharmacy = listNewlyDiscoveredData
                .Where(x => x.Key == MedicalReportKey.NewPharmacy)
                .ToList();
            var listNewPerformances = listNewlyDiscoveredData
                .Where(x => x.Key == MedicalReportKey.NewPerformances)
                .ToList();

            foreach (var diseases in listNewDiseases)
            {
                if (medicalReportDTO.UserReport?.UserId > 0 && diseases.DiseasePharmacyId.HasValue)
                {
                    listUserDiseases.Add(
                        new UserDisease()
                        {
                            UserId = medicalReportDTO.UserReport.UserId,
                            DiseaseId = diseases.DiseasePharmacyId ?? 0,
                            Note = diseases.NoteMined,
                            StartDateString = diseases.DateMined
                        }
                    );
                }
            }

            foreach (var newPharmacy in listNewPharmacy)
            {
                if (
                    medicalReportDTO.UserReport?.UserId > 0
                    && newPharmacy.DiseasePharmacyId.HasValue
                )
                {
                    listUserPharmacy.Add(
                        new UserPharmacy()
                        {
                            Name = newPharmacy.ItemMined,
                            UserId = medicalReportDTO.UserReport.UserId,
                            PharmacyId = newPharmacy.DiseasePharmacyId,
                            Dosing = newPharmacy.DoseMined,
                            LastUpdateUser = (await CurrentUserAsync(medicalReportDTO.UserReport.UserId))?.FullName
                        }
                    );
                }
            }

            foreach (var newPerformances in listNewPerformances)
            {
                if (
                    medicalReportDTO.UserReport?.UserId > 0
                    && newPerformances.DiseasePharmacyId.HasValue
                )
                {
                    listUserNonpharmacy.Add(
                        new UserNonpharmacy()
                        {
                            UserId = medicalReportDTO.UserReport.UserId,
                            NonpharmaticTherapyId = newPerformances.DiseasePharmacyId ?? 0,
                            Note = newPerformances.NoteMined
                        }
                    );
                }
            }
            return (listUserDiseases, listUserPharmacy, listUserNonpharmacy);
        }

        private async Task<UserDTO> CurrentUserAsync(int userId)
        {

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return null;

            return _mapper.Map<User, UserDTO>(user);
        }

        private List<UserAnamnesis> GetListUserAnamnesis(
            int userId,
            string lastUserUpdate,
            List<UserAnamnesis> listUserAnamnesis,
            List<PatientMedicalExaminationDTO> healthStatus
        )
        {
            var listNewUserAnamnesis = new List<UserAnamnesis>();

            foreach (var item in healthStatus)
            {
                if (item.Key == MedicalReportKey.Height)
                {
                    ProcessUserAnamnesis(
                        userId,
                        lastUserUpdate,
                        listUserAnamnesis,
                        listNewUserAnamnesis,
                        item.ItemMined,
                        (int)PersonalAnamnesisType.Height
                    );
                }
                else if (item.Key == MedicalReportKey.Weight)
                {
                    ProcessUserAnamnesis(
                        userId,
                        lastUserUpdate,
                        listUserAnamnesis,
                        listNewUserAnamnesis,
                        item.ItemMined,
                        (int)PersonalAnamnesisType.Weight
                    );
                }
                else if (item.Key == MedicalReportKey.BloodPressure)
                {
                    ProcessUserAnamnesis(
                        userId,
                        lastUserUpdate,
                        listUserAnamnesis,
                        listNewUserAnamnesis,
                        item.ItemMined,
                        (int)PersonalAnamnesisType.BloodPressure
                    );
                }
                else if (item.Key == MedicalReportKey.Pulse)
                {
                    ProcessUserAnamnesis(
                        userId,
                        lastUserUpdate,
                        listUserAnamnesis,
                        listNewUserAnamnesis,
                        item.ItemMined,
                        (int)PersonalAnamnesisType.HeartRate
                    );
                }
                else if (item.Key == MedicalReportKey.LDL)
                {
                    ProcessUserAnamnesis(
                        userId,
                        lastUserUpdate,
                        listUserAnamnesis,
                        listNewUserAnamnesis,
                        item.ItemMined,
                        (int)PersonalAnamnesisType.Cholesterol
                    );
                }
            }

            return listNewUserAnamnesis;
        }

        private void ProcessUserAnamnesis(
            int userId,
            string lastUserUpdate,
            List<UserAnamnesis> listUserAnamnesis,
            List<UserAnamnesis> listNewUserAnamnesis,
            string personalValue,
            int personalType
        )
        {
            var userAnamnesis = listUserAnamnesis.FirstOrDefault(
                x => x.IsPersonal_Type == personalType
            );
            if (userAnamnesis == null)
            {
                listNewUserAnamnesis.Add(
                    new UserAnamnesis()
                    {
                        IsPersonalAnamnesis = true,
                        IsPersonal_Date = DateTime.Now,
                        IsPersonal_Type = personalType,
                        IsPersonal_Value = personalValue,
                        IsPersonal_CreatorType = PersonalAnamnesisCreatorType.User,
                        UserId = userId,
                        LastUpdateUser = lastUserUpdate
                    }
                );
            }
            else
            {
                userAnamnesis.IsPersonal_Value = personalValue;
                userAnamnesis.LastUpdateUser = lastUserUpdate;
            }
        }
        private string GetEnumDescription(Enum value)
        {
            var feild = value.GetType().GetField(value.ToString());

            if (feild != null)
            {
                DescriptionAttribute[] attributes = feild.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

                if (attributes != null && attributes.Any())
                {
                    return attributes.First().Description;
                }
            }
            return value.ToString();
        }
        #endregion
    }
}
