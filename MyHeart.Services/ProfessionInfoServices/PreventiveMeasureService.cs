using AutoMapper;
using MyHeart.Data;
using MyHeart.Data.Models;
using MyHeart.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MyHeart.DTO.ProfessionalInformation;
using MyHeart.DTO;
using System;
using MyHeart.Data.Models.ProfessionInformation;

namespace MyHeart.Services
{
    public class PreventiveMeasureService : IPreventiveMeasureService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly MyHeartContext _myHeartContext;
        private readonly IConfiguration _configuration;

        private readonly IEmailService _emailService;

        public PreventiveMeasureService(IHttpContextAccessor httpContextAccessor, MyHeartContext gradologyContext, IMapper mapper, IConfiguration configuration, IEmailService emailService)
        {
            _httpContextAccessor = httpContextAccessor;
            _myHeartContext = gradologyContext;
            _mapper = mapper;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<IEnumerable<PreventiveMeasuresDTO>> PreventiveMeasuresListAsync()
        {
            var preventiveMeasures = await _myHeartContext.PreventiveMeasures.ToListAsync();

            return _mapper.Map<IEnumerable<PreventiveMeasures>, IEnumerable<PreventiveMeasuresDTO>>(preventiveMeasures);
        }

        public async Task<DataTableResponse<PreventiveMeasuresDTO>> GetPreventiveMeasuresTable(DataTableRequest request)
        {
            List<PreventiveMeasures> symptoms;
            int totalCount;

            //no need to search all strings if there is no filter
            if (!string.IsNullOrEmpty(request.Filter))
            {
                //no need to filter twice
                var filtered = _myHeartContext.PreventiveMeasures
                    .Where(x => x.Name.ToLower().Contains(request.Filter.ToLower()));

                symptoms = await filtered
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                totalCount = await filtered
                    .CountAsync();
            }
            else
            {
                symptoms = await _myHeartContext.PreventiveMeasures
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                totalCount = await _myHeartContext.PreventiveMeasures
                    .CountAsync();
            }

            return new DataTableResponse<PreventiveMeasuresDTO>
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                Data = _mapper.Map<IEnumerable<PreventiveMeasures>, IEnumerable<PreventiveMeasuresDTO>>(symptoms).ToList()
            };
        }

        public async Task<DataTableResponse<PreventiveMeasuresDTO>> GetPagedResource(SortedPagedSourceRequest request)
        {
            List<PreventiveMeasures> symptoms;
            int totalCount;
            var selected = request.Selected.ToList();
            var clientPages = (int)Math.Ceiling((double)selected.Count / request.PageSize);
            var firstPageOffset = selected.Count % request.PageSize;

            var measures = _myHeartContext.PreventiveMeasures.AsQueryable();

            if (!string.IsNullOrEmpty(request.Filter))
            {
                measures = measures
                    .Where(x => x.Name.ToLower().Contains(request.Filter.ToLower()));

                symptoms = await measures
                        .Skip((request.Page - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .ToListAsync();

                totalCount = (await measures.CountAsync()) + selected.Count;
            }
            else
            {
                if (request.Selected.Count() > 0)
                {
                    measures = measures
                        .Where(x => !selected.Contains(x.Id));
                }

                if (request.Page < clientPages || (request.Page == clientPages && firstPageOffset == 0))
                {
                    symptoms = new List<PreventiveMeasures>();
                }
                else if (request.Page == clientPages)
                {
                    symptoms = await measures
                        .Take(request.PageSize - firstPageOffset)
                        .ToListAsync();
                }
                else
                {
                    symptoms = await measures
                        .Skip((request.Page - clientPages - 1) * request.PageSize + (clientPages == 0 ? 0 : (request.PageSize - firstPageOffset)))
                        .Take(request.PageSize)
                        .ToListAsync();
                }

                totalCount = (await measures.CountAsync()) + selected.Count;
            }


            return new DataTableResponse<PreventiveMeasuresDTO>
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                Data = _mapper.Map<IEnumerable<PreventiveMeasures>, IEnumerable<PreventiveMeasuresDTO>>(symptoms).ToList()
            };
        }

        public async Task<IEnumerable<PreventiveMeasuresDTO>> GetByIds(IEnumerable<int> ids)
        {
            var measures = await _myHeartContext.PreventiveMeasures
                .Where(x => ids.Contains(x.Id)).ToListAsync();

            return _mapper.Map<IEnumerable<PreventiveMeasures>, IEnumerable<PreventiveMeasuresDTO>>(measures);
        }

        public async Task<PreventiveMeasuresDTO> PreventiveMeasuresAsync(int preventiveMeasuresId)
        {
            var preventiveMeasures = await _myHeartContext.PreventiveMeasures
                .Where(d => d.Id == preventiveMeasuresId)
                .Include(x => x.WorkspaceList)
                .Include(p => p.Indication)
                .Include(x => x.Synonyms)
                .FirstOrDefaultAsync();

            return _mapper.Map<PreventiveMeasures, PreventiveMeasuresDTO>(preventiveMeasures);
        }

        public async Task<PreventiveMeasuresDTO> UpdatePreventiveMeasures(PreventiveMeasuresDTO preventiveMeasures, string user)
        {
            var dbPreventiveMeasures = await _myHeartContext.PreventiveMeasures
                .Where(u => u.Id == preventiveMeasures.Id)
                .Include(x => x.WorkspaceList)
                .Include(p => p.Indication)
                .Include(x => x.Synonyms)
                .FirstOrDefaultAsync();

            if (dbPreventiveMeasures == null)
            {
                return null;
            }

            dbPreventiveMeasures.Name = preventiveMeasures.Name;
            dbPreventiveMeasures.Description = preventiveMeasures.Description;
            dbPreventiveMeasures.LastUpdateUser = user;

            #region disease bond
            int bondCount = dbPreventiveMeasures.Indication == null ? 0 : dbPreventiveMeasures.Indication.Count;
            List<int> idList = new List<int>();
            List<Disease_PreventiveMeasures_PreventiveMeasures> indicList = new List<Disease_PreventiveMeasures_PreventiveMeasures>();
            var lastContraIndication = await _myHeartContext.Disease_PreventiveMeasures_PreventiveMeasures.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            int lastId = lastContraIndication == null ? 0 : lastContraIndication.Id;
            int itemId = 1;
            foreach (var item in preventiveMeasures.Indication)
            {
                if (item.Id == 0)
                {
                    if (lastContraIndication != null)
                    {
                        lastId++;
                        item.Id = lastId;
                    }
                    else
                    {
                        item.Id = itemId;
                        itemId++;
                    }
                    bool exist = false;
                    foreach (var existItem in dbPreventiveMeasures.Indication)
                    {
                        if (existItem.DiseaseId == item.DiseaseId && existItem.PreventiveMeasuresId == item.PreventiveMeasuresId)
                        {
                            idList.Add(existItem.Id);
                            exist = true;
                        }
                    }

                    if (exist)
                    {
                        indicList.Add(_mapper.Map<Disease_PreventiveMeasures_PreventiveMeasures>(item));
                    }
                    else
                    {
                        var bond = _mapper.Map<Disease_PreventiveMeasures_PreventiveMeasures>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.Disease_PreventiveMeasures_PreventiveMeasures.AddAsync(bond);
                        idList.Add(item.Id);
                    }

                }
                else
                {
                    indicList.Add(_mapper.Map<Disease_PreventiveMeasures_PreventiveMeasures>(item));
                    idList.Add(item.Id);
                }
            }
            if (bondCount != 0)
            {
                foreach (var item in dbPreventiveMeasures.Indication)
                {
                    foreach (var indication in indicList)
                    {
                        if (indication.DiseaseId == item.DiseaseId && indication.PreventiveMeasuresId == item.PreventiveMeasuresId)
                        {
                            //item.Id = symptom.Id;
                            item.BondStrength = indication.BondStrength;
                            item.LastUpdateUser = user;
                        }

                    }
                    if (!idList.Contains(item.Id))
                    {
                        _myHeartContext.Disease_PreventiveMeasures_PreventiveMeasures.Remove(item);
                    }
                }
            }
            #endregion

            #region medicalFacilities bond
            bondCount = dbPreventiveMeasures.WorkspaceList == null ? 0 : dbPreventiveMeasures.WorkspaceList.Count;
            idList = new List<int>();
            List<MedicalFacilities_PreventiveMeasures_Preventive> workspaceList = new List<MedicalFacilities_PreventiveMeasures_Preventive>();
            var lastMedicalFacilities = await _myHeartContext.MedicalFacilities_PreventiveMeasures_Preventive.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastId = lastMedicalFacilities == null ? 0 : lastMedicalFacilities.Id;
            itemId = 1;
            foreach (var item in preventiveMeasures.WorkspaceList)
            {
                if (item.Id == 0)
                {
                    if (lastMedicalFacilities != null)
                    {
                        lastId++;
                        item.Id = lastId;
                    }
                    else
                    {
                        item.Id = itemId;
                        itemId++;
                    }
                    bool exist = false;
                    foreach (var existItem in dbPreventiveMeasures.WorkspaceList)
                    {
                        if (existItem.MedicalFacilitiesId == item.MedicalFacilitiesId && existItem.PreventiveMeasureId == item.PreventiveMeasureId)
                        {
                            idList.Add(existItem.Id);
                            exist = true;
                        }
                    }

                    if (exist)
                    {
                        workspaceList.Add(_mapper.Map<MedicalFacilities_PreventiveMeasures_Preventive>(item));
                    }
                    else
                    {
                        var bond = _mapper.Map<MedicalFacilities_PreventiveMeasures_Preventive>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.MedicalFacilities_PreventiveMeasures_Preventive.AddAsync(bond);
                        idList.Add(item.Id);
                    }

                }
                else
                {
                    workspaceList.Add(_mapper.Map<MedicalFacilities_PreventiveMeasures_Preventive>(item));
                    idList.Add(item.Id);
                }
            }
            if (bondCount != 0)
            {
                foreach (var item in dbPreventiveMeasures.WorkspaceList)
                {
                    foreach (var workspace in workspaceList)
                    {
                        if (workspace.MedicalFacilitiesId == item.MedicalFacilitiesId && workspace.PreventiveMeasureId == item.PreventiveMeasureId)
                        {
                            //item.Id = symptom.Id;
                            item.BondStrength = workspace.BondStrength;
                            item.LastUpdateUser = user;
                        }

                    }
                    if (!idList.Contains(item.Id))
                    {
                        _myHeartContext.MedicalFacilities_PreventiveMeasures_Preventive.Remove(item);
                    }
                }
            }
            #endregion

            #region update synonyms

            foreach (var dbSynonym in dbPreventiveMeasures.Synonyms)
            {
                var exists = preventiveMeasures.Synonyms.Any(x => x.Id == dbSynonym.Id);
                if (!exists)
                {
                    _myHeartContext.Remove(dbSynonym);
                }
            }

            foreach (var synonym in preventiveMeasures.Synonyms)
            {
                if (synonym.Id == 0)
                {
                    var dbSynonym = _mapper.Map<PreventiveMeasures_Synonyms>(synonym);
                    dbSynonym.PreventiveMeasureId = preventiveMeasures.Id;
                    _myHeartContext.Add(dbSynonym);
                }
                else
                {
                    var existingSynonym = dbPreventiveMeasures.Synonyms.FirstOrDefault(x => x.Id == synonym.Id);
                    if (existingSynonym != null)
                    {
                        existingSynonym.Name = synonym.Name;
                    }
                }
            }

            #endregion

            await _myHeartContext.SaveChangesAsync();

            return _mapper.Map<PreventiveMeasuresDTO>(preventiveMeasures);
        }

        public async Task<PreventiveMeasuresDTO> AddPreventiveMeasuresAsync(PreventiveMeasuresDTO preventiveMeasures, string user)
        {
            var dbpreventiveMeasures = new PreventiveMeasures()
            {
                Name = preventiveMeasures.Name,
                Description = preventiveMeasures.Description,
                LastUpdateUser = user,
                Synonyms = _mapper.Map<List<PreventiveMeasures_Synonyms>>(preventiveMeasures.Synonyms)
            };

            _myHeartContext.Add(dbpreventiveMeasures);

            await _myHeartContext.SaveChangesAsync();

            #region disease bond
            var lastContraIndication = await _myHeartContext.Disease_PreventiveMeasures_PreventiveMeasures.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            int lastId = lastContraIndication == null ? 0 : lastContraIndication.Id;
            int itemId = 1;
            if (preventiveMeasures.Indication != null)
            {
                foreach (var item in preventiveMeasures.Indication)
                {
                    if (item.Id == 0)
                    {
                        if (lastContraIndication != null)
                        {
                            lastId++;
                            item.Id = lastId;
                        }
                        else
                        {
                            item.Id = itemId;
                            itemId++;
                        }
                        item.PreventiveMeasuresId = dbpreventiveMeasures.Id;
                        var bond = _mapper.Map<Disease_PreventiveMeasures_PreventiveMeasures>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.Disease_PreventiveMeasures_PreventiveMeasures.AddAsync(bond);
                    }
                }
            }

            #endregion

            #region medicalFacilities bond
            var lastMedicalFacilities = await _myHeartContext.MedicalFacilities_PreventiveMeasures_Preventive.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastId = lastMedicalFacilities == null ? 0 : lastMedicalFacilities.Id;
            itemId = 1;
            if (preventiveMeasures.WorkspaceList != null)
            {
                foreach (var item in preventiveMeasures.WorkspaceList)
                {
                    if (item.Id == 0)
                    {
                        if (lastMedicalFacilities != null)
                        {
                            lastId++;
                            item.Id = lastId;
                        }
                        else
                        {
                            item.Id = itemId;
                            itemId++;
                        }
                        item.PreventiveMeasureId = dbpreventiveMeasures.Id;
                        var bond = _mapper.Map<MedicalFacilities_PreventiveMeasures_Preventive>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.MedicalFacilities_PreventiveMeasures_Preventive.AddAsync(bond);
                    }
                }
            }

            #endregion

            await _myHeartContext.SaveChangesAsync();

            return _mapper.Map<PreventiveMeasures, PreventiveMeasuresDTO>(dbpreventiveMeasures);
        }

        public async Task<PreventiveMeasuresDTO> DeletePreventiveMeasures(int preventiveMeasuresId)
        {
            var dbpreventiveMeasures = await _myHeartContext.PreventiveMeasures.FirstOrDefaultAsync(u => u.Id == preventiveMeasuresId);

            if (dbpreventiveMeasures == null)
            {
                return null;
            }

            _myHeartContext.PreventiveMeasures.Remove(dbpreventiveMeasures);

            await _myHeartContext.SaveChangesAsync();

            return _mapper.Map<PreventiveMeasuresDTO>(dbpreventiveMeasures);
        }


        public async Task<Dictionary<string, string>> ValidatePreventiveMeasures(PreventiveMeasuresDTO therapyNews)
        {
            var errorList = new Dictionary<string, string>();

            return errorList;
        }
    }
}