using MyHeart.Data;
using MyHeart.DTO;
using MyHeart.DTO.Questions;
using MyHeart.DTO.User;
using MyHeart.Services.Interfaces;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyHeart.Services
{
    public class StatisticsService : IStatisticsService
    {
        private MyHeartContext _myHeartContext;

        private IUserService _userService;
        private ISubscriptionService _subscriptionService;
        private IQuestionService _questionService;
        private IDiseaseService _diseaseService;
        private INonpharmacyService _nonpharmacyService;
        private IPharmacyService _pharmacyService;
        private IAnamnesisService _anamnesisService;

        public StatisticsService(MyHeartContext myHeartContext, IUserService userService, ISubscriptionService subscriptionService, IQuestionService questionService, IDiseaseService diseaseService, INonpharmacyService nonpharmacyService, IPharmacyService pharmacyService, IAnamnesisService anamnesisService)
        {
            _myHeartContext = myHeartContext;
            _userService = userService;
            _subscriptionService = subscriptionService;
            _questionService = questionService;
            _diseaseService = diseaseService;
            _nonpharmacyService = nonpharmacyService;
            _pharmacyService = pharmacyService;
            _anamnesisService = anamnesisService;
        }

        public async Task<FileDTO> ExportPacients()
        {
            var users = await _userService.GetForPatientsExport();
            var subscriptions = await _subscriptionService.GetSubscriptions();

            DataTable pacientsTable = CreatePatientsTable(users, subscriptions);

            string fileName = "Pacienti";

            byte[] fileContent = ExportToExcel(pacientsTable, fileName);

            return CreateFileDTO(fileName, fileContent);
        }

        public async Task<FileDTO> ExportQuestions()
        {
            var questionList = await _questionService.GetListFullAsync();
            var subscriptions = await _subscriptionService.GetSubscriptions();

            DataTable questionTable = CreateQuestionTable(questionList, subscriptions);

            string fileName = "Dotazy";

            byte[] fileContent = ExportToExcel(questionTable, fileName);

            return CreateFileDTO(fileName, fileContent);
        }

        public async Task<FileDTO> ExportDoctors()
        {
            var doctors = await _userService.GetDoctorsForExport();

            var doctorIds = doctors
                .Select(d => d.Id)
                .ToArray();

            var questionsComments = await _questionService.GetClosedQuestionCommentsByDoctorIds(doctorIds);

            var doctorTable = CreateDoctorsTable(doctors, questionsComments);

            string fileName = "Doktoři";

            byte[] fileContent = ExportToExcel(doctorTable, fileName);

            return CreateFileDTO(fileName, fileContent);
        }

        public async Task<FileDTO> ExportDiseases()
        {
            var userDiseases = await _diseaseService.GetAllUserDiseases();

            DataTable diseasseTable = CreateDiseaseTable(userDiseases);

            string fileName = "Onemocnění";

            byte[] fileContent = ExportToExcel(diseasseTable, fileName);

            return CreateFileDTO(fileName, fileContent);
        }

        public async Task<FileDTO> ExportPharmacy()
        {
            var userPharmacy = await _anamnesisService.GetAllPharmacyAnamnesis();

            DataTable pharmacyTable = CreatePharmacyTable(userPharmacy);

            string fileName = "Léčba";

            byte[] fileContent = ExportToExcel(pharmacyTable, fileName);

            return CreateFileDTO(fileName, fileContent);
        }

        public async Task<FileDTO> ExportNonpharmacy()
        {
            var userPharmacy = await _nonpharmacyService.GetAllUserNonpharmacy();

            DataTable pharmacyTable = CreateNonpharmacyTable(userPharmacy);

            string fileName = "Provedený výkon";

            byte[] fileContent = ExportToExcel(pharmacyTable, fileName);

            return CreateFileDTO(fileName, fileContent);
        }

        private DataTable CreateNonpharmacyTable(IEnumerable<UserNonpharmacyDto> userNonpharmacies)
        {
            DataTable table = new DataTable();

            table.Columns.Add("Název", typeof(string));
            table.Columns.Add("Počet uživatelů s provedeným výkonem", typeof(int));

            var nonpharmacyGroups = userNonpharmacies.GroupBy(x => x.NonpharmaticTherapyId);

            foreach (var nonpharmacyGroup in nonpharmacyGroups)
            {
                var nonpharmaticTherapy = nonpharmacyGroup.First().NonpharmaticTherapy;

                var nonpharmacyCount = nonpharmacyGroup
                    .Select(n => n.UserId)
                    .Distinct()
                    .Count();

                table.Rows.Add(nonpharmaticTherapy.Name, nonpharmacyCount);
            }

            return table;
        }

        private DataTable CreatePharmacyTable(IEnumerable<UserAnamnesisDTO> userPharmacy)
        {
            DataTable table = new DataTable();

            table.Columns.Add("Název", typeof(string));
            table.Columns.Add("Počet uživatelů s léčbou", typeof(int));

            var pharmacyGroups = userPharmacy.GroupBy(x => x.IsPharmacy_Name);

            foreach (var pharmacyGroup in pharmacyGroups)
            {
                var userPharmacyCount = pharmacyGroup
                    .Select(x => x.UserId)
                    .Distinct()
                    .Count();

                table.Rows.Add(pharmacyGroup.Key, userPharmacyCount);
            }

            return table;
        }

        private DataTable CreateDiseaseTable(IEnumerable<UserDiseaseDto> userDiseases)
        {
            DataTable table = new DataTable();

            table.Columns.Add("Název", typeof(string));
            table.Columns.Add("Počet uživatelů s onemocněním", typeof(int));

            var diseasseGroups = userDiseases.GroupBy(x => x.DiseaseId);

            foreach (var diseaseGroup in diseasseGroups)
            {
                var diseasse = diseaseGroup.First().Disease;
                var userDiseaseCount = diseaseGroup
                    .Select(x => x.UserId)
                    .Distinct()
                    .Count();

                table.Rows.Add(diseasse.Name, userDiseaseCount);
            }

            return table;
        }

        private DataTable CreateDoctorsTable(IEnumerable<UserFullDTO> doctors, IEnumerable<QuestionCommentDTO> questionsComments)
        {
            DataTable table = new DataTable();

            table.Columns.Add("Jméno", typeof(string));
            table.Columns.Add("Počet uzavřených dotazů", typeof(int));

            foreach (var doctor in doctors)
            {
                var closedQuestionCount = questionsComments
                    .GroupBy(c => c.QuestionId)
                    .Where(x => x.Any(c => c.SenderId == doctor.Id))
                    .Count();
                    
                table.Rows.Add($"{doctor.FirstName} {doctor.LastName}", closedQuestionCount);
            }

            return table;
        }

        private DataTable CreateQuestionTable(IEnumerable<QuestionFullDto> questions, IEnumerable<SubscriptionDTO> subscriptions)
        {
            var table = new DataTable();

            table.Columns.Add("Druh předplatného", typeof(string));
            table.Columns.Add("Datum založení", typeof(string));
            table.Columns.Add("Datum uzavření", typeof(string));
            table.Columns.Add("Počet hodin od založení do uzavření", typeof(int));
            table.Columns.Add("Odpovídající lékař", typeof(string));

            foreach (var question in questions)
            {
                UserFullDTO user = question.User;
                string subscriptionName = GetSubscriptionName(subscriptions, user.UserDetail?.SubscriptionId);

                double? dialogDurationHours = null;
                string lastUpdateDate = "";

                if (question.Status == QuestionStatus.Closed)
                {
                    dialogDurationHours = (question.LastUpdateDate - question.CreatedAt).Hours;
                    lastUpdateDate = question.LastUpdateDate.ToString();
                }

                var doctorNamesList = question.Comments
                    .Where(c => c.SenderId != question.UserId)
                    .Select(c => $"{c.Sender.FirstName} {c.Sender.LastName}")
                    .Distinct();

                var doctorNamesListString = string.Join(';', doctorNamesList);

                table.Rows.Add(subscriptionName, question.CreationDate, lastUpdateDate, dialogDurationHours, doctorNamesListString);
            }

            return table;
        }

        private DataTable CreatePatientsTable(IEnumerable<UserFullDTO> users, IEnumerable<SubscriptionDTO> subscriptions)
        {
            DataTable table = new DataTable();

            table.Columns.Add("Jméno", typeof(string));
            table.Columns.Add("Ověřená identita", typeof(string));
            table.Columns.Add("Datum registrace", typeof(string));
            table.Columns.Add("Druh předplatného", typeof(string));
            table.Columns.Add("Počet onemocnění", typeof(int));
            table.Columns.Add("Počet medikací", typeof(int));
            table.Columns.Add("Počet dotazů", typeof(int));


            foreach (var user in users)
            {
                string defaultSubscriptionName = GetSubscriptionName(subscriptions, user.UserDetail?.SubscriptionId);

                table.Rows.Add($"{user.FirstName} {user.LastName}", user.EmailComfirmed ? "Ano" : "Ne", user.Created, defaultSubscriptionName, user.UserDiseases.Count, user.UserNonpharmacy.Count, user.Questions.Count);
            }

            return table;
        }

        private FileDTO CreateFileDTO(string filename, byte[] fileContent)
        {
            var now = DateTime.Now;

            return new FileDTO
            {
                Name = $"{filename}_{now:ddMMyyyy_hhmmss}",
                Content = fileContent,
                Extension = ".xlsx",
                MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };
        }

        private string GetSubscriptionName(IEnumerable<SubscriptionDTO> subscriptions, int? subscriptionId)
        {
            string defaultSubscriptionName = subscriptions.First().Name;

            if (subscriptionId == null)
                return defaultSubscriptionName;

            var subscription = subscriptions.FirstOrDefault(s => s.Id == subscriptionId);

            return subscription.Name;
        }

        private byte[] ExportToExcel(DataTable dataTable, string sheetName)
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetName);

                // Write column headers
                for (int i = 1; i <= dataTable.Columns.Count; i++)
                {
                    worksheet.Cells[1, i].Value = dataTable.Columns[i - 1].ColumnName;
                }

                // Write data
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        worksheet.Cells[i + 2, j + 1].Value = dataTable.Rows[i][j];
                    }
                }

                // Save the Excel file
                return package.GetAsByteArray();
            }
        }

        public byte[] ExportToExcelNew<T>(IList<T> listExport, FileInfo file)
        {
            using (var package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                PropertyInfo[] properties = typeof(T).GetProperties();

                int index = 0;
                foreach (PropertyInfo property in properties)
                {
                    var attribute = property.GetCustomAttributes(typeof(MappingExcel), false)
                        .FirstOrDefault() as MappingExcel;

                    if (attribute != null)
                    {
                        string columnName = attribute.ColumnName;
                        worksheet.Cells[1, index + 1].Value = columnName;
                        index++;
                    }
                }

                for (int i = 0; i < listExport.Count; i++)
                {
                    var export = listExport[i];
                    if(export == null) continue;

                    for (int j = 0; j < properties.Length; j++)
                    {
                        var range = worksheet.Cells[i + 2, j + 1];

                        if (properties[j].PropertyType == typeof(DateTime) || properties[j].PropertyType == typeof(DateTime?))
                        {
                            range.Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                        }

                        if (properties[j].PropertyType == typeof(decimal) || properties[j].PropertyType == typeof(decimal?))
                        {
                            range.Style.Numberformat.Format = "0.00";
                        }
                        range.Value = properties[j].GetValue(export);
                    }
                }
                return package.GetAsByteArray();
            }
        }
    }
}
