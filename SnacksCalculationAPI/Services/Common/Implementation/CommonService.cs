using OfficeOpenXml.Style;
using OfficeOpenXml;
using SnacksCalculationAPI.Services.Common.Interfaces;
using SnacksCalculationAPI.Services.FileUtility.Implementation;

namespace SnacksCalculationAPI.Services.Common.Implementation
{
    public class CommonService :ICommonService
    {
        private readonly APIDbContext _context;

        public CommonService(APIDbContext context)
        {
            _context = context;
        }
        private void SetTableStyle(ExcelWorksheet workSheet, int columnCount)
        {
            workSheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.TabColor = System.Drawing.Color.Black;
            workSheet.DefaultRowHeight = 14;
        }

        private static void InsertHeaders(List<string> headers, ExcelWorksheet workSheet)
        {
            for (int i = 0; i < headers.Count; i++)
            {
                workSheet.Cells[1, i + 1].Value = headers[i];
            }
        }

        public Task<FileData> GetMonthlyDetailsExcel(string fromDate, string toDate)
        {
            throw new NotImplementedException();
        }
    }
}
