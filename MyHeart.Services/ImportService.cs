using AutoMapper;
using MyHeart.Data;
using MyHeart.Data.Models;
using MyHeart.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyHeart.Services
{
    public class ImportService : IImportService
    {
        private readonly IMapper _mapper;
        private readonly MyHeartContext _myHeartContext;

        public ImportService(MyHeartContext myHeartContext, IMapper mapper)
        {
            _myHeartContext = myHeartContext;
            _mapper = mapper;
        }

        public async Task<bool> ImportData(IFormFile file)
        {

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int rowIndex = 2; rowIndex <= rowCount; rowIndex++)
                    {
                        var row = new ImportSchema();

                        row.Name = worksheet.Cells[rowIndex, 1].Value?.ToString().Trim();

                    }
                }
            }

            await _myHeartContext.SaveChangesAsync();

            await _myHeartContext.SaveChangesAsync();

            await _myHeartContext.SaveChangesAsync();

            return true;
        }

    }
}

public class ImportSchema
{
    public string Name { get; set; }

}