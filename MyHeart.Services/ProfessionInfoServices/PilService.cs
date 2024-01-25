using AutoMapper;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MoreLinq;
using MyHeart.Data;
using MyHeart.Data.Models.ProfessionInformation;
using MyHeart.DTO;
using MyHeart.DTO.ProfessionInformation;
using MyHeart.Services.Interfaces.Azure;
using MyHeart.Services.Interfaces.ProfessionInfo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHeart.Services.ProfessionInfoServices {
    public class PilService : IPilservice {
        MyHeartContext _context;
        IConfiguration _config;
        IBlob _blob;
        IMapper _mapper;

        public PilService(MyHeartContext context, IConfiguration configuration, IBlob blob, IMapper mapper) {
            _config = configuration;
            _context = context;
            _blob = blob;
            _mapper = mapper;
        }

        public async Task<(bool changed, string oldPath)> AddOrUpdatePil(PilDTO pil, string user) {
            var dbPil = await _context.Pils.Where(x => x.SuklCode == pil.SuklCode).FirstOrDefaultAsync();
            var shouldchange = false;
            var old = "";

            if(dbPil == null) {
                dbPil = new Pil();
                shouldchange = true;
                dbPil.FilePath = pil.FilePath;
                dbPil.SuklCode = pil.SuklCode;
                dbPil.OnWeb = pil.OnWeb;
                dbPil.DataUpdated = pil.DataUpdated;
                dbPil.LastUpdateUser = user;

                _context.Pils.Add(dbPil);
            } else {
                dbPil.LastUpdateUser = user;
                if (dbPil.FilePath != pil.FilePath) {
                    shouldchange = true;
                    if (!dbPil.OnWeb) {
                        old = dbPil.FilePath;
                    }
                    dbPil.FilePath = pil.FilePath;
                    dbPil.OnWeb = pil.OnWeb;
                    dbPil.DataUpdated = pil.DataUpdated;
                }else if (dbPil.DataUpdated != pil.DataUpdated) {
                    old = dbPil.FilePath;
                    dbPil.DataUpdated = pil.DataUpdated;
                    shouldchange = true;
                }

                _context.Pils.Update(dbPil);
            }

            _context.SaveChanges();

            return (shouldchange, old);
        }

        public async Task<(Stream file, string name)> GetFileStreamBySukl(int sukl) {
            var pil = await _context.Pils.Where(x => x.SuklCode == sukl).FirstOrDefaultAsync();

            if (pil == null || pil.OnWeb) {
                return (null, null);
            }

            var file = await _blob.GetDownloadFileStream(pil.FilePath);

            return (file, _mapper.Map<Pil, PilDTO>(pil).GetFileName());
        }

        public async Task<PilResponse> GetPillBySukl(int sukl) {
            var res = new PilResponse();
            var pil = await _context.Pils.Where(x => x.SuklCode == sukl).FirstOrDefaultAsync();

            if(pil == null) {
                res.PilAvailable = false;
            } else {
                res.Sukl = pil.SuklCode;
                res.PilAvailable = true;
                res.PilOnWeb = pil.OnWeb;
                if (pil.OnWeb) {
                    res.PilURL = pil.FilePath;
                } else {
                    res.PilFileName = pil.FilePath;
                }
            }

            return res;
        }

        public async Task<bool> BulkInsertOrUpdate(List<PilDTO> pils, string user) {
            try {
                var dbPil = _mapper.Map<List<PilDTO>, List<Pil>>(pils);

                foreach (var pil in dbPil) {
                    pil.LastUpdateUser = user;
                }

                var batches = dbPil.Batch(2000);

                await _context.SetIdentityInsertAsync<Pil>(true);
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
                await _context.SetIdentityInsertAsync<Pil>(false);

                return true;
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e);
                return false;
            }
        }
        public async Task<bool> BulkDelete(List<PilDTO> pils) {
            try {
                var dbPils = _mapper.Map<List<PilDTO>, List<Pil>>(pils);

                var batches = dbPils.Batch(2000);

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
