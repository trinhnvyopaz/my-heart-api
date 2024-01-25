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
using EFCore.BulkExtensions;
using MoreLinq;

namespace MyHeart.Services
{
    public class MedicalFacilitieService : IMedicalFacilitieService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly MyHeartContext _myHeartContext;
        private readonly IConfiguration _configuration;

        private readonly IEmailService _emailService;

        public MedicalFacilitieService(IHttpContextAccessor httpContextAccessor, MyHeartContext gradologyContext, IMapper mapper, IConfiguration configuration, IEmailService emailService)
        {
            _httpContextAccessor = httpContextAccessor;
            _myHeartContext = gradologyContext;
            _mapper = mapper;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<IEnumerable<MedicalFacilitiesDTO>> MedicalFacilitiesListAsync()
        {
            var medicalFacilities = await _myHeartContext.MedicalFacilities.ToListAsync();

            return _mapper.Map<IEnumerable<MedicalFacilities>, IEnumerable<MedicalFacilitiesDTO>>(medicalFacilities);
        }

        public async Task<MedicalFacilitiesDTO> MedicalFacilitiesAsync(int medicalFacilitiesId)
        {
            var medicalFacilities = await _myHeartContext.MedicalFacilities
                .Where(d => d.Id == medicalFacilitiesId)
                .Include(x => x.PreventiveMeasures)
                .Include(p => p.NonpharmaticTherapy)
                .FirstOrDefaultAsync();

            return _mapper.Map<MedicalFacilities, MedicalFacilitiesDTO>(medicalFacilities);
        }

        public async Task<DataTableResponse<MedicalFacilitiesDTO>> GetMedicalFacilitiesTable(DataTableRequest request) {
            List<MedicalFacilities> symptoms;
            int totalCount;

            //no need to search all strings if there is no filter
            if (!string.IsNullOrEmpty(request.Filter)) {
                //no need to filter twice
                var filtered = _myHeartContext.MedicalFacilities
                    .Where(x => x.Name.ToLower().Contains(request.Filter.ToLower()));

                symptoms = await filtered
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                totalCount = await filtered
                    .CountAsync();
            } else {
                symptoms = await _myHeartContext.MedicalFacilities
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                totalCount = await _myHeartContext.MedicalFacilities
                    .CountAsync();
            }

            return new DataTableResponse<MedicalFacilitiesDTO> {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                Data = _mapper.Map<IEnumerable<MedicalFacilities>, IEnumerable<MedicalFacilitiesDTO>>(symptoms).ToList()
            };
        }

        public async Task<DataTableResponse<MedicalFacilitiesDTO>> GetPagedResource(SortedPagedSourceRequest request) {
            List<MedicalFacilities> diseases;
            int totalCount;
            var selected = request.Selected.ToList();
            var clientPages = (int)Math.Ceiling((double)selected.Count / request.PageSize);
            var firstPageOffset = selected.Count % request.PageSize;

            var dis = _myHeartContext.MedicalFacilities.AsQueryable();

            //no need to search all strings if there is no filter
            if (!string.IsNullOrEmpty(request.Filter)) {
                //no need to filter twice
                dis = dis
                    .Where(x => x.Name.ToLower().Contains(request.Filter.ToLower()));

                diseases = await dis
                        .Skip((request.Page - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .ToListAsync();

                totalCount = (await dis.CountAsync());
            } else {
                if (request.Selected.Count() > 0) {
                    dis = dis
                        .Where(x => !selected.Contains(x.Id));
                }

                if (request.Page < clientPages || (request.Page == clientPages && firstPageOffset == 0)) {
                    diseases = new List<MedicalFacilities>();
                } else if (request.Page == clientPages) {
                    diseases = await dis
                        .Take(request.PageSize - firstPageOffset)
                        .ToListAsync();
                } else {
                    diseases = await dis
                        .Skip((request.Page - clientPages - 1) * request.PageSize + (clientPages == 0 ? 0 : (request.PageSize - firstPageOffset)))
                        .Take(request.PageSize)
                        .ToListAsync();
                }

                totalCount = (await dis.CountAsync()) + selected.Count;
            }

            return new DataTableResponse<MedicalFacilitiesDTO> {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                Data = _mapper.Map<IEnumerable<MedicalFacilities>, IEnumerable<MedicalFacilitiesDTO>>(diseases).ToList()
            };
        }

        public async Task<IEnumerable<MedicalFacilitiesDTO>> GetByIds(IEnumerable<int> ids) {
            var disease = await _myHeartContext.MedicalFacilities
                .Where(x => ids.Contains(x.Id)).ToListAsync();

            return _mapper.Map<IEnumerable<MedicalFacilities>, IEnumerable<MedicalFacilitiesDTO>>(disease);
        }

        public async Task<MedicalFacilitiesDTO> UpdateMedicalFacilities(MedicalFacilitiesDTO medicalFacilities, string user)
        {
            var dbmedicalFacilities = await _myHeartContext.MedicalFacilities
                            .Where(u => u.Id == medicalFacilities.Id)
                            .Include(x => x.PreventiveMeasures)
                            .Include(p => p.NonpharmaticTherapy)
                            .FirstOrDefaultAsync();

            if (dbmedicalFacilities == null)
            {
                return null;
            }

            dbmedicalFacilities.Name = medicalFacilities.Name;
            dbmedicalFacilities.Character = medicalFacilities.Character;
            dbmedicalFacilities.Email = medicalFacilities.Email;
            dbmedicalFacilities.Telephone = medicalFacilities.Telephone;
            dbmedicalFacilities.LastUpdateUser = user;

            #region preventive bond
            int bondCount = dbmedicalFacilities.PreventiveMeasures == null ? 0 : dbmedicalFacilities.PreventiveMeasures.Count;
            List<int> idList = new List<int>();
            List<MedicalFacilities_PreventiveMeasures_Preventive> preventiveList = new List<MedicalFacilities_PreventiveMeasures_Preventive>();
            var lastPreventive = await _myHeartContext.MedicalFacilities_PreventiveMeasures_Preventive.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            int lastId = lastPreventive == null ? 0 : lastPreventive.Id;
            int itemId = 1;
            foreach (var item in medicalFacilities.PreventiveMeasures)
            {
                if (item.Id == 0)
                {
                    if (lastPreventive != null)
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
                    foreach (var existItem in dbmedicalFacilities.PreventiveMeasures)
                    {
                        if (existItem.PreventiveMeasureId == item.PreventiveMeasureId && existItem.MedicalFacilitiesId == item.MedicalFacilitiesId)
                        {
                            idList.Add(existItem.Id);
                            exist = true;
                        }
                    }

                    if (exist)
                    {
                        preventiveList.Add(_mapper.Map<MedicalFacilities_PreventiveMeasures_Preventive>(item));
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
                    preventiveList.Add(_mapper.Map<MedicalFacilities_PreventiveMeasures_Preventive>(item));
                    idList.Add(item.Id);
                }
            }
            if (bondCount != 0)
            {
                foreach (var item in dbmedicalFacilities.PreventiveMeasures)
                {
                    foreach (var preventive in preventiveList)
                    {
                        if (preventive.MedicalFacilitiesId == item.MedicalFacilitiesId && preventive.PreventiveMeasureId == item.PreventiveMeasureId)
                        {
                            //item.Id = symptom.Id;
                            item.BondStrength = preventive.BondStrength;
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

            #region nonpharmacy bond
            bondCount = dbmedicalFacilities.NonpharmaticTherapy == null ? 0 : dbmedicalFacilities.NonpharmaticTherapy.Count;
            idList = new List<int>();
            List<NonpharmaticTherapy_MedicalFacilities_MedicalIntervention> nonpharmacyList = new List<NonpharmaticTherapy_MedicalFacilities_MedicalIntervention>();
            var lastSideEffect = await _myHeartContext.NonpharmaticTherapy_MedicalFacilities_MedicalIntervention.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastId = lastSideEffect == null ? 0 : lastSideEffect.Id;
            itemId = 1;
            foreach (var item in medicalFacilities.NonpharmaticTherapy)
            {
                if (item.Id == 0)
                {
                    if (lastSideEffect != null)
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
                    foreach (var existItem in dbmedicalFacilities.NonpharmaticTherapy)
                    {
                        if (existItem.NonpharmaticTherapyId == item.NonpharmaticTherapyId && existItem.MedicalFacilitiesId == item.MedicalFacilitiesId)
                        {
                            idList.Add(existItem.Id);
                            exist = true;
                        }
                    }

                    if (exist)
                    {
                        nonpharmacyList.Add(_mapper.Map<NonpharmaticTherapy_MedicalFacilities_MedicalIntervention>(item));
                    }
                    else
                    {
                        var bond = _mapper.Map<NonpharmaticTherapy_MedicalFacilities_MedicalIntervention>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.NonpharmaticTherapy_MedicalFacilities_MedicalIntervention.AddAsync(bond);
                        idList.Add(item.Id);
                    }

                }
                else
                {
                    nonpharmacyList.Add(_mapper.Map<NonpharmaticTherapy_MedicalFacilities_MedicalIntervention>(item));
                    idList.Add(item.Id);
                }
            }
            if (bondCount != 0)
            {
                foreach (var item in dbmedicalFacilities.NonpharmaticTherapy)
                {
                    foreach (var nonpharmatic in nonpharmacyList)
                    {
                        if (nonpharmatic.NonpharmaticTherapyId == item.NonpharmaticTherapyId && nonpharmatic.MedicalFacilitiesId == item.MedicalFacilitiesId)
                        {
                            //item.Id = symptom.Id;
                            item.BondStrength = nonpharmatic.BondStrength;
                            item.LastUpdateUser = user;
                        }

                    }
                    if (!idList.Contains(item.Id))
                    {
                        _myHeartContext.NonpharmaticTherapy_MedicalFacilities_MedicalIntervention.Remove(item);
                    }
                }
            }
            #endregion

            await _myHeartContext.SaveChangesAsync();

            return _mapper.Map<MedicalFacilitiesDTO>(medicalFacilities);
        }

        public async Task<MedicalFacilitiesDTO> AddMedicalFacilitiesAsync(MedicalFacilitiesDTO medicalFacilities, string user)
        {
            var dbmedicalFacilities = new MedicalFacilities() {
                Name = medicalFacilities.Name,
                Character = medicalFacilities.Character,
                Telephone = medicalFacilities.Telephone,
                Email = medicalFacilities.Email,
                LastUpdateUser = user
            };

            _myHeartContext.Add(dbmedicalFacilities);

            await _myHeartContext.SaveChangesAsync();

            #region preventive bond
            var lastPreventive = await _myHeartContext.MedicalFacilities_PreventiveMeasures_Preventive.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            int lastId = lastPreventive == null ? 0 : lastPreventive.Id;
            int itemId = 1;
            if (medicalFacilities.PreventiveMeasures != null)
            {
                foreach (var item in medicalFacilities.PreventiveMeasures)
                {
                    if (item.Id == 0)
                    {
                        if (lastPreventive != null)
                        {
                            lastId++;
                            item.Id = lastId;
                        }
                        else
                        {
                            item.Id = itemId;
                            itemId++;
                        }
                        item.MedicalFacilitiesId = dbmedicalFacilities.Id;
                        var bond = _mapper.Map<MedicalFacilities_PreventiveMeasures_Preventive>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.MedicalFacilities_PreventiveMeasures_Preventive.AddAsync(bond);
                    }
                }
            }
            
            #endregion

            #region nonpharmacy bond
            var lastSideEffect = await _myHeartContext.NonpharmaticTherapy_MedicalFacilities_MedicalIntervention.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            lastId = lastSideEffect == null ? 0 : lastSideEffect.Id;
            itemId = 1;
            if (medicalFacilities.NonpharmaticTherapy != null)
            {
                foreach (var item in medicalFacilities.NonpharmaticTherapy)
                {
                    if (item.Id == 0)
                    {
                        if (lastSideEffect != null)
                        {
                            lastId++;
                            item.Id = lastId;
                        }
                        else
                        {
                            item.Id = itemId;
                            itemId++;
                        }
                        item.MedicalFacilitiesId = dbmedicalFacilities.Id;
                        var bond = _mapper.Map<NonpharmaticTherapy_MedicalFacilities_MedicalIntervention>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.NonpharmaticTherapy_MedicalFacilities_MedicalIntervention.AddAsync(bond);
                    }
                }
            }
            
            #endregion

            await _myHeartContext.SaveChangesAsync();

            return _mapper.Map<MedicalFacilities, MedicalFacilitiesDTO>(dbmedicalFacilities);
        }

        public async Task<bool> AddOrUpdateMedicalFacility(MedicalFacilitiesDTO facility, string user) {
            var dbFacility = await _myHeartContext.MedicalFacilities.Where(x => x.FacilityId == facility.FacilityId).FirstOrDefaultAsync();
            var found = true;

            if(dbFacility == null) {
                dbFacility = new MedicalFacilities();
                found = false;
            }

            dbFacility.FacilityId = facility.FacilityId;
            dbFacility.Name = facility.Name;
            dbFacility.FacilityCode = facility.FacilityCode;
            dbFacility.FacilityTypeCode = facility.FacilityTypeCode;
            dbFacility.Character = facility.Character;
            dbFacility.CharacterSecondary = facility.CharacterSecondary;
            dbFacility.ICO = facility.ICO;
            dbFacility.PCZ = facility.PCZ;
            dbFacility.PCDP = facility.PCDP;
            dbFacility.Region = facility.Region;
            dbFacility.RegionCode = facility.RegionCode;
            dbFacility.District = facility.District;
            dbFacility.DistrictCode = facility.DistrictCode;
            dbFacility.AdministrativeDistrict = facility.AdministrativeDistrict;
            dbFacility.Municipality = facility.Municipality;
            dbFacility.ZIP = facility.ZIP;
            dbFacility.Street = facility.Street;
            dbFacility.BuildingNumber = facility.BuildingNumber;
            dbFacility.Email = facility.Email;
            dbFacility.Fax = facility.Fax;
            dbFacility.Telephone = facility.Telephone;
            dbFacility.Website = facility.Website;
            dbFacility.ProviderType = facility.ProviderType;
            dbFacility.ProviderName = facility.ProviderName;
            dbFacility.CareField = facility.CareField;
            dbFacility.CareForm = facility.CareForm;
            dbFacility.CareType = facility.CareType;
            dbFacility.Representative = facility.Representative;
            dbFacility.GPS = facility.GPS;
            dbFacility.LastUpdateUser = user;

            if (found) {
                _myHeartContext.Update(dbFacility);
            } else {
                await _myHeartContext.AddAsync(dbFacility);
            }

            await _myHeartContext.SaveChangesAsync();

            return true;
        }

        public async Task<MedicalFacilitiesDTO> DeleteMedicalFacilities(int medicalFacilitiesId)
        {
            var dbmedicalFacilities = await _myHeartContext.MedicalFacilities.FirstOrDefaultAsync(u => u.Id == medicalFacilitiesId);

            if (dbmedicalFacilities == null)
            {
                return null;
            }

            _myHeartContext.MedicalFacilities.Remove(dbmedicalFacilities);

            await _myHeartContext.SaveChangesAsync();

            return _mapper.Map<MedicalFacilitiesDTO>(dbmedicalFacilities);
        }

        public async Task<bool> BulkInsertOrUpdate(List<MedicalFacilitiesDTO> facilities, string user) {
            try {
                var dbFacility = _mapper.Map<List<MedicalFacilitiesDTO>, List<MedicalFacilities>>(facilities);

                foreach (var facility in dbFacility) {
                    facility.LastUpdateUser = user;
                }

                var batches = dbFacility.Batch(2000);

                await _myHeartContext.SetIdentityInsertAsync<MedicalFacilities>(true);
                foreach(var batch in batches) {
                    var done = false;
                    var i = 0;
                    while (!done) {
                        await Task.Delay(TimeSpan.FromMinutes(5 * i));
                        try {
                            await _myHeartContext.BulkInsertOrUpdateAsync(batch.ToList(), new BulkConfig { SqlBulkCopyOptions = Microsoft.Data.SqlClient.SqlBulkCopyOptions.KeepIdentity, BatchSize = 500, BulkCopyTimeout = 0 });
                            done = true;
                        } catch (Exception) {
                            i++;
                        }
                    }
                }
                await _myHeartContext.SetIdentityInsertAsync<MedicalFacilities>(false);

                return true;
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> BulkDelete(List<MedicalFacilitiesDTO> facilities) {
            try {
                var dbFacilities = _mapper.Map<List<MedicalFacilitiesDTO>, List<MedicalFacilities>>(facilities);

                var batches = dbFacilities.Batch(2000);

                foreach(var batch in batches) {
                    var done = false;
                    var i = 0;
                    while (!done) {
                        await Task.Delay(TimeSpan.FromMinutes(5 * i));
                        try {
                            await _myHeartContext.BulkDeleteAsync(batch.ToList(), new BulkConfig { SqlBulkCopyOptions = Microsoft.Data.SqlClient.SqlBulkCopyOptions.KeepIdentity, BatchSize = 500, BulkCopyTimeout = 0 });
                            done = true;
                        } catch (Exception) {
                            i++;
                        }
                    }
                }

                return true;
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e);
                return false;
            }
        }

        #region Validation

        public async Task<Dictionary<string, string>> ValidateMedicalFacilities(MedicalFacilitiesDTO medicalFacilities)
        {
            var errorList = new Dictionary<string, string>();

            return errorList;
        }

        #endregion
    }

}