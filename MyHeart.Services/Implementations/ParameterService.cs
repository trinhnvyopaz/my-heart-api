using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyHeart.Data;
using MyHeart.Data.Models;
using MyHeart.DTO;
using MyHeart.DTO.MedicalReport;
using MyHeart.Services.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyHeart.Services
{
    public class ParameterService : IParameterService
    {
        private readonly MyHeartContext _myHeartContext;
        private readonly IMapper _mapper;

        public ParameterService(MyHeartContext myHeartContext, IMapper mapper)
        {
            _myHeartContext = myHeartContext;
            _mapper = mapper;
        }
        public async Task<List<ParameterDTO>> GetListAsync()
        {
            var listParameter = await _myHeartContext.Parameter.Include(x => x.Markers).ToListAsync();

            return _mapper.Map<IEnumerable<Parameter>, List<ParameterDTO>>(listParameter);
        }

        public async Task<ParameterDTO> CreateParameterAsync(ParameterDTO parameterDTO)
        {
            if (parameterDTO == null)
                return null;

            var dbModel = _mapper.Map<Parameter>(parameterDTO);

            _myHeartContext.Update(dbModel);

            await _myHeartContext.SaveChangesAsync();

            return _mapper.Map<ParameterDTO>(dbModel);
        }
        public async Task<ParameterDTO> UpdateParameterAsync(ParameterDTO parameterDTO)
        {
            var dbModel = await _myHeartContext.Parameter.FirstOrDefaultAsync(u => u.Id == parameterDTO.Id);

            if (dbModel == null)
                return null;

            _myHeartContext.Update(dbModel);

            await _myHeartContext.SaveChangesAsync();

            return _mapper.Map<ParameterDTO>(dbModel);
        }
        public async Task<ParameterDTO> DeleteParameterAsync(int id)
        {
            var dbModel = await _myHeartContext.Parameter.FirstOrDefaultAsync(u => u.Id == id);

            if (dbModel == null)
                return null;

            await _myHeartContext.SaveChangesAsync();

            return _mapper.Map<ParameterDTO>(dbModel);
        }
        public async Task<List<ParameterDTO>> ExtractDataOCR(string ocrText)
        {
            try
            {
                if (string.IsNullOrEmpty(ocrText))
                    return null;

                string replaceOCRText = Regex.Replace(ocrText, @"\t|\n|\r", "");
                string regexCheckExits = @"({0})";
                //string regexDate = @"(?<={0})\s(\d{1,2}.\d{1,2}.\d{4})";
                //string regexDate = @"(?<={0})\s+(.+?)(\b(\d{2})[-/.](\d{2})[-/.](\d{4})\b)";
                string regexKnownData = @"({0})\s*(.+?)\s*(?=:)";
                string regexKnownIllness = @"({0})\s*(.+?)\s*(\sO\b|\sAA\b|\sneguje\b|\sFA\b)";
                //string regexHeath = @"(?<={0})(.+?)(cm|kg|mmHg|\d+/min|mg/dL|mmol/l)\b";
                string regexPharmacy = @"({0})\s*(.+?)({1})(.+?)(:)";
                string regexDiscontinued = @"(?<=\sdrop\b|\sEX\b|\snepoužívat\b)\s*(.+?)\s*(,|.)";
                string regexChar = @"({0})(.{{{1}}})";

                var listParameterDTO = await GetListAsync();

                foreach (var parameterDTO in listParameterDTO)
                {
                    string beforeMarker = string.Join("|", parameterDTO.Markers.Where(x => x.MakerGroup == MakerGroup.Before)
                        .Select(c => $@"\s{c.Value}\b"));

                    //if (string.IsNullOrEmpty(beforeMarker)) continue;

                    var ocrRegex = new Regex(string.Format(regexCheckExits, beforeMarker), RegexOptions.IgnoreCase);
                    bool isMatch = ocrRegex.IsMatch(replaceOCRText);
                    if (!isMatch) continue;

                    if (parameterDTO.Type == MedicalReportType.MessageType &&
                        replaceOCRText.Length > parameterDTO.SearchType)
                    {
                        if (string.IsNullOrEmpty(parameterDTO.Keywords))
                        {
                            parameterDTO.Value = "0";
                            continue;
                        }
                        //string pattern = @"(\sAmbulantní\b|\slékařská\b|\skontrola\b|\svyšetření\b|\szpráva\b)";
                        string pattern = $"({string.Join("|", parameterDTO.Keywords.Split(",").Select(c => $@"\s{c}\b"))})";
                        Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
                        Match matchMessageType = regex.Match(replaceOCRText);
                        parameterDTO.Value = matchMessageType.Success ? "1" : "0";
                        continue;
                    }
                    else if (parameterDTO.Type == MedicalReportType.MessageDate && replaceOCRText.Length > parameterDTO.SearchType)
                    {
                        parameterDTO.Value = RegexMessageDate(replaceOCRText, beforeMarker);
                        continue;
                    }
                    else if (parameterDTO.Type == MedicalReportType.PersonalData
                        && parameterDTO.Key == MedicalReportKey.Workplace
                        && replaceOCRText.Length > parameterDTO.SearchType)
                    {
                        parameterDTO.Value = replaceOCRText.Substring(0, parameterDTO.SearchType);
                        continue;
                    }
                    else if (parameterDTO.Type == MedicalReportType.KnownData && parameterDTO.Key == MedicalReportKey.KnownIllness)
                    {
                        ocrRegex = new Regex(string.Format(regexKnownIllness, beforeMarker), RegexOptions.IgnoreCase);
                    }
                    else if (parameterDTO.Type == MedicalReportType.KnownData && parameterDTO.Key == MedicalReportKey.AWellKnownPharmacy)
                    {
                        ocrRegex = new Regex(string.Format(regexPharmacy, beforeMarker, MedicalReportConsts.PharmacyDose), RegexOptions.IgnoreCase);
                    }
                    else if (parameterDTO.Type == MedicalReportType.KnownData)
                    {
                        ocrRegex = new Regex(string.Format(regexKnownData, beforeMarker), RegexOptions.IgnoreCase);
                    }
                    else if (parameterDTO.Type == MedicalReportType.NewlyDiscoveredData &&
                        parameterDTO.Key == MedicalReportKey.DiscontinuedTreatment)
                    {
                        ocrRegex = new Regex(regexDiscontinued, RegexOptions.IgnoreCase);
                    }
                    else if (parameterDTO.Type == MedicalReportType.HealthStatus)
                    {
                        ocrRegex = new Regex(string.Format(regexChar, beforeMarker, parameterDTO.SearchType), RegexOptions.IgnoreCase);
                    }
                    else if (parameterDTO.Type == MedicalReportType.PersonalData || parameterDTO.Type == MedicalReportType.HealthStatus)
                    {
                        ocrRegex = new Regex(string.Format(regexChar, beforeMarker, parameterDTO.SearchType), RegexOptions.IgnoreCase);
                    }

                    MatchCollection matches = ocrRegex.Matches(replaceOCRText);
                    parameterDTO.BeforeMarkerReally = matches.FirstOrDefault()?.Groups[1].Value;
                    parameterDTO.Value = string.Join(" ", matches);

                    if (!string.IsNullOrEmpty(parameterDTO.BeforeMarkerReally))
                        parameterDTO.Value = parameterDTO.Value.Replace(parameterDTO.BeforeMarkerReally, string.Empty);

                    //Match match = ocrRegex.Match(replaceOCRText);
                    //if (parameterDTO.Type == MedicalReportType.MessageDate || parameterDTO.Type == MedicalReportType.HealthStatus)
                    //{
                    //    MatchCollection matches = ocrRegex.Matches(replaceOCRText);
                    //    parameterDTO.BeforeMarkerReally = matches.FirstOrDefault()?.Groups[1].Value;
                    //    parameterDTO.Value = string.Join(" ", matches);
                    //}
                    //else if (match.Groups.Count > 2)
                    //{
                    //    parameterDTO.BeforeMarkerReally = match.Groups[1].Value;
                    //    parameterDTO.Value = string.Join("", match.Groups.Cast<Group>().Skip(2).Select(g => g.Value));
                    //}
                }
                return listParameterDTO;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string RegexMessageDate(string replaceOCRText, string beforeMarker)
        {
            string regexDateFullText = @"(?<={0}).*?\b(\d{1,2}\.\d{1,2}\.\d{4}(\s| - )\d{2}:\d{2})\b|\b(\d{1,2}\.+\d{1,2}\.+\d{4})\b";
            string regexDate = @"(\d{1,2}\.\d{1,2}\.\d{4}(\s| - )\d{2}:\d{2})|(\d{1,2}\.+\d{1,2}\.+\d{4})";
            var ocrRegex = new Regex(regexDateFullText.Replace("{0}", beforeMarker), RegexOptions.IgnoreCase);

            MatchCollection matchMessageDateText = ocrRegex.Matches(replaceOCRText.Substring(0, Math.Min(replaceOCRText.Length, 1000)));
            var matchMessageDate = Regex.Matches(string.Join(",", matchMessageDateText), regexDate)
                                        .Cast<Match>()
                                        .Select(m => m.Value)
                                        .ToList();
            if (matchMessageDate.Count == 0) return string.Empty;

            List<DateTime> listDate = new List<DateTime>();

            foreach (var dateStr in matchMessageDate)
            {
                if (DateTime.TryParseExact(dateStr, "dd.MM.yyyy - mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime outDate1))
                    listDate.Add(outDate1);
                if (DateTime.TryParseExact(dateStr, "dd.MM.yyyy mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime outDate2))
                    listDate.Add(outDate2);
                else if (DateTime.TryParseExact(dateStr, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime outDate3))
                    listDate.Add(outDate3);
            }

            DateTime currentDate = DateTime.Now.Date;
            var minDate = listDate.Select(date => Math.Abs((date - currentDate).TotalDays)).Min();
            int index = listDate.FindIndex(date => Math.Abs((date - currentDate).TotalDays) == minDate);

            return matchMessageDate[index];
        }
    }
}
