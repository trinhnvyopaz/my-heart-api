using AutoMapper;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using MyHeart.Data;
using MyHeart.Data.Models.ProfessionInformation;
using MyHeart.DTO;
using MyHeart.DTO.ProfessionInformation;
using MyHeart.Services.Interfaces.ProfessionInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services.ProfessionInfoServices
{
    public class AtcService : IAtcService
    {

        private readonly MyHeartContext _context;
        private readonly IMapper _mapper;

        public AtcService(MyHeartContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper;
        }

        public async Task<IEnumerable<AtcDTO>> GetAllAtcs()
        {
            var result = await _context.Atc
                .ToListAsync();

            return _mapper.Map<IEnumerable<Atc>, IEnumerable<AtcDTO>>(result);
        }

        public async Task<DataTableResponse<AtcDTO>> GetPagedResource(SortedPagedSourceRequest request) {
            List<Atc> atc;
            int totalCount;
            var selected = request.Selected.ToList();
            var clientPages = (int)Math.Ceiling((double)selected.Count / request.PageSize);
            var firstPageOffset = selected.Count % request.PageSize;

            var atcs = _context.Atc.AsQueryable();

            //no need to search all strings if there is no filter
            if (!string.IsNullOrEmpty(request.Filter)) {
                //no need to filter twice
                atcs = atcs
                    .Where(x => x.AtcCode.ToLower().StartsWith(request.Filter.ToLower()));

                atc = await atcs
                        .Skip((request.Page - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .ToListAsync();

                totalCount = (await atcs.CountAsync());
            } else {
                if (request.Selected.Count() > 0) {
                    atcs = atcs
                        .Where(x => !selected.Contains(x.Id));
                }

                if (request.Page < clientPages || (request.Page == clientPages && firstPageOffset == 0)) {
                    atc = new List<Atc>();
                } else if (request.Page == clientPages) {
                    atc = await atcs
                        .Take(request.PageSize - firstPageOffset)
                        .ToListAsync();
                } else {
                    atc = await atcs
                        .Skip((request.Page - clientPages - 1) * request.PageSize + (clientPages == 0 ? 0 : (request.PageSize - firstPageOffset)))
                        .Take(request.PageSize)
                        .ToListAsync();
                }

                totalCount = (await atcs.CountAsync()) + selected.Count;
            }

            

            return new DataTableResponse<AtcDTO> {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                Data = _mapper.Map<IEnumerable<Atc>, IEnumerable<AtcDTO>>(atc).ToList()
            };
        }

        public async Task<IEnumerable<AtcDTO>> GetByIds(IEnumerable<int> ids) {
            var disease = await _context.Atc
                .Where(x => ids.Contains(x.Id)).ToListAsync();

            return _mapper.Map<IEnumerable<Atc>, IEnumerable<AtcDTO>>(disease);
        }

        public async Task<bool> AddOrUpdateAtc(AtcDTO atc, string user) {
            var dbAtc = await _context.Atc.Where(x => x.AtcCode == atc.AtcCode).FirstOrDefaultAsync();
            var found = true;

            if(dbAtc == null) {
                dbAtc = new Atc();
                found = false;
            }

            dbAtc.AtcCode = atc.AtcCode;
            dbAtc.NT = atc.NT;
            dbAtc.Name = atc.Name;
            dbAtc.NameEn = atc.NameEn;
            dbAtc.LastUpdateUser = user;

            if (found) {
                _context.Update(dbAtc);
            } else {
                await _context.AddAsync(dbAtc);
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> BulkInsertOrUpdate(List<AtcDTO> atcList, string user) {
            try {
                var dbAtc = _mapper.Map<List<AtcDTO>, List<Atc>>(atcList);

                foreach (var atc in dbAtc) {
                    atc.LastUpdateUser = user;
                }

                var batches = dbAtc.Batch(2000);

                await _context.SetIdentityInsertAsync<Atc>(true);
                foreach(var batch in batches) {
                    var done = false;
                    var i = 0;
                    while (!done) {
                        await Task.Delay(TimeSpan.FromMinutes(5 * i));
                        try {
                            await _context.BulkInsertOrUpdateAsync(batch.ToList(), new BulkConfig { SqlBulkCopyOptions = Microsoft.Data.SqlClient.SqlBulkCopyOptions.KeepIdentity, BatchSize = 500, BulkCopyTimeout = 0 });
                            done = true;
                        } catch (Exception) {
                            i++;
                        }
                    }
                }
                await _context.SetIdentityInsertAsync<Atc>(false);

                return true;
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e);
                return false;
            }
        }
        public async Task<bool> BulkDelete(List<AtcDTO> atcs) {
            try {
                var dbAtcs = _mapper.Map<List<AtcDTO>, List<Atc>>(atcs);

                var batches = dbAtcs.Batch(2000);

                foreach(var batch in batches) {
                    var done = false;
                    var i = 0;
                    while (!done) {
                        await Task.Delay(TimeSpan.FromMinutes(5 * i));
                        try {
                            await _context.BulkDeleteAsync(batch.ToList(), new BulkConfig { SqlBulkCopyOptions = Microsoft.Data.SqlClient.SqlBulkCopyOptions.KeepIdentity, BatchSize = 500, BulkCopyTimeout = 0 });
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
    }
}
