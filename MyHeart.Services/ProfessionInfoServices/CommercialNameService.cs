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

namespace MyHeart.Services
{
    public class CommercialNameService : ICommercialNameService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly MyHeartContext _myHeartContext;
        private readonly IConfiguration _configuration;

        private readonly IEmailService _emailService;

        public CommercialNameService(IHttpContextAccessor httpContextAccessor, MyHeartContext gradologyContext, IMapper mapper, IConfiguration configuration, IEmailService emailService)
        {
            _httpContextAccessor = httpContextAccessor;
            _myHeartContext = gradologyContext;
            _mapper = mapper;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<IEnumerable<CommercialNameDTO>> CommercialNamesListAsync()
        {
            var commercialNames = await _myHeartContext.CommercialName.ToListAsync();

            return _mapper.Map<IEnumerable<CommercialName>, IEnumerable<CommercialNameDTO>>(commercialNames);
        }
        public async Task<CommercialNameDTO> CommercialNamesAsync(int commercialNamesId)
        {
            var commercialNames = await _myHeartContext.CommercialName
                .Where(d => d.Id == commercialNamesId)
                .Include(x => x.Pharmacy)
                .FirstOrDefaultAsync();

            return _mapper.Map<CommercialName, CommercialNameDTO>(commercialNames);
        }

        public async Task<CommercialNameDTO> UpdateCommercialNames(CommercialNameDTO commercialNames, string user)
        {
            var dbCommercialNames = await _myHeartContext.CommercialName
                .Where(d => d.Id == commercialNames.Id)
                .Include(x => x.Pharmacy)
                .FirstOrDefaultAsync();

            if (dbCommercialNames == null)
            {
                return null;
            }

            dbCommercialNames.Name = commercialNames.Name;
            dbCommercialNames.LastUpdateUser = user;

            //update symtopms bond
            #region update Symptoms
            int bondCount = dbCommercialNames.Pharmacy.Count;
            List<int> idList = new List<int>();
            var lastPharmacy = await _myHeartContext.CommercialName_Pharmacy.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            int lastId = lastPharmacy == null ? 0 : lastPharmacy.Id;
            int itemId = 1;
            foreach (var item in commercialNames.Pharmacy)
            {
                if (item.Id == 0)
                {
                    if (lastPharmacy != null)
                    {
                        lastId++;
                        item.Id = lastId;
                    }
                    else
                    {
                        item.Id = itemId;
                        itemId++;
                    }
                    var bond = _mapper.Map<CommercialName_Pharmacy>(item);
                    bond.LastUpdateUser = user;
                    await _myHeartContext.CommercialName_Pharmacy.AddAsync(bond);
                    idList.Add(item.Id);
                }
                else
                {
                    idList.Add(item.Id);
                }
            }
            if (bondCount != 0)
            {
                foreach (var item in dbCommercialNames.Pharmacy)
                {
                    if (!idList.Contains(item.Id))
                    {
                        _myHeartContext.CommercialName_Pharmacy.Remove(item);
                    }
                }
            }
            #endregion

            await _myHeartContext.SaveChangesAsync();

            return _mapper.Map<CommercialNameDTO>(commercialNames);
        }



        public async Task<CommercialNameDTO> AddCommercialNamesAsync(CommercialNameDTO commercialNames, string user)
        {

            var dbCommercialNames = new CommercialName() {
                Name = commercialNames.Name,
                LastUpdateUser = user
            };

            _myHeartContext.Add(dbCommercialNames);

            await _myHeartContext.SaveChangesAsync();

            #region update Symptoms
            var lastSymptom = await _myHeartContext.CommercialName_Pharmacy.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            int lastId = lastSymptom == null ? 0 : lastSymptom.Id;
            int itemId = 1;

            if (commercialNames.Pharmacy != null)
            {
                foreach (var item in commercialNames.Pharmacy)
                {
                    if (item.Id == 0)
                    {
                        if (lastSymptom != null)
                        {
                            lastId++;
                            item.Id = lastId;
                        }
                        else
                        {
                            item.Id = itemId;
                            itemId++;
                        }
                        item.CommercialNamesId = dbCommercialNames.Id;
                        var bond = _mapper.Map<CommercialName_Pharmacy>(item);
                        bond.LastUpdateUser = user;
                        await _myHeartContext.CommercialName_Pharmacy.AddAsync(bond);
                    }
                }
            }
            
            #endregion

                await _myHeartContext.SaveChangesAsync();
            return _mapper.Map<CommercialName, CommercialNameDTO>(dbCommercialNames);
        }

        public async Task<CommercialNameDTO> DeleteCommercialNames(int CommercialNamesId)
        {
            var CommercialNames = await _myHeartContext.CommercialName.FirstOrDefaultAsync(u => u.Id == CommercialNamesId);

            if (CommercialNames == null)
            {
                return null;
            }

            _myHeartContext.CommercialName.Remove(CommercialNames);

            await _myHeartContext.SaveChangesAsync();

            return _mapper.Map<CommercialNameDTO>(CommercialNames);
        }


        #region Validation

        public async Task<Dictionary<string, string>> ValidateCommercialNames(CommercialNameDTO CommercialNames)
        {
            var errorList = new Dictionary<string, string>();

            if (CommercialNames.Name.Length < 2)
                errorList.Add("Name", "Název nesmí být kratší než 2 znaky");

            return errorList;
        }

        #endregion

    }
}