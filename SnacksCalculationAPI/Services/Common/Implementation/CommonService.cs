using OfficeOpenXml.Style;
using OfficeOpenXml;
using SnacksCalculationAPI.Services.Common.Interfaces;
using SnacksCalculationAPI.Services.FileUtility.Implementation;
using SnacksCalculationAPI.Services.FileUtility.Interfaces;

namespace SnacksCalculationAPI.Services.Common.Implementation
{
    public class CommonService :ICommonService
    {
        private readonly APIDbContext _context;
        private IFileService _file;

        public CommonService(APIDbContext context, IFileService file)
        {
            _context = context;
            _file = file;
        }
        private void SetTableStyle(ExcelWorksheet workSheet, int columnCount)
        {
            workSheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.TabColor = System.Drawing.Color.Black;
            workSheet.DefaultRowHeight = 14;
        }

    
        public async Task<FileData> GetMonthlyDetailsExcel(string fromDate, string toDate)
        {
          //  var result = null;
            

            ExcelPackage excel = new ExcelPackage();

            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
            var headers = new List<string>();
            headers=["Date","Super Admin","Admin","Normal User","Items"];

            _file.SetTableStyle(workSheet, headers.Count);
            _file.SetHeaderStyle(workSheet, headers.Count);
            _file.InsertHeaders(headers, workSheet);
           // Insert_AuditReportExcelRows2(result, workSheet);
            _file.AutoExcelFitColumns(headers.Count, workSheet);

            FileData fileData = _file.GetFileData(excel.GetAsByteArray());
            excel.Dispose();
             return fileData;
        }
    }
}
