using OfficeOpenXml.Style;
using OfficeOpenXml;
using SnacksCalculationAPI.Services.Common.Interfaces;
using SnacksCalculationAPI.Services.FileUtility.Implementation;
using SnacksCalculationAPI.Services.FileUtility.Interfaces;
using Microsoft.EntityFrameworkCore;
using SnacksCalculationAPI.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
            var listQuery = from cm in _context.CostModels
                            join um in _context.UserModels on cm.UserId equals um.Id
                            where string.Compare(cm.Date, fromDate) >= 0 && string.Compare(cm.Date, toDate) <= 0
                            orderby cm.UserId ascending

                            select new UserCostModel
                            {
                                Id = cm.Id,
                                UserId = cm.UserId,
                                Date = cm.Date,
                                Amount = cm.Amount,
                                Item = cm.Item,
                            };
            var result = await listQuery.ToListAsync();
            var userQuery = _context.UserModels.AsQueryable();
            /*
            var sumByUserId = _context.UserInformationModels
                    .GroupBy(t => t.UserId)
                    .Select(g => new UserCostData
                    {
                        UserId = g.Key,
                        Amount = g.Sum(t => t.Amount)
                    })
                    .ToList();
            
            */
   

   
          
            var userList = await userQuery.ToListAsync();
            ExcelPackage excel = new ExcelPackage();

            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
            var headers = new List<string>();
            var headersId = new List<int>();
            headers = ["Date"];
           for(int index=0;index<userList.Count;index++)
            {
                headers.Insert(index + 1, userList[index].Name);
                headersId.Insert(index, userList[index].Id);
            }
            headers.Insert(userList.Count + 1, "Items"); 

            _file.SetTableStyle(workSheet, headers.Count);
            _file.SetHeaderStyle(workSheet, headers.Count);
            _file.InsertHeaders(headers, workSheet);
             Insert_AuditReportExcelRows2(toDate, headersId, result, workSheet);
            _file.AutoExcelFitColumns(headers.Count, workSheet);

            FileData fileData = _file.GetFileData(excel.GetAsByteArray());
            excel.Dispose();
             return fileData;
        }


        private void Insert_AuditReportExcelRows2(string toDate,List<int> headersId, List<UserCostModel> result, ExcelWorksheet workSheet)
        {
            var perUserTotalAmount = _context.UserInformationModels
                   .GroupBy(t => t.UserId)
                   .Select(g => new UserCostData
                   {
                       UserId = g.Key,
                       Amount = g.Sum(t => t.Amount)
                   })
                   .ToList();

            int startingRowNumberForData = 2;
            var date = toDate.Substring(0, 8);
            var rowNumber= Int32.Parse(toDate.Substring(8, 2));
            List<double> perUserTotalCost = new List<double>();
            for (int i = 0; i < headersId.Count; i++)
            {
                perUserTotalCost.Insert(i, 0);
            }

            int column;
            for (int row=0;row< rowNumber; row++) {
                 column = 1;
                var rowVal = (row + 1);
               var  day = rowVal.ToString();
                if(day.Length==1)day = "0"+day;
                var dateValue = date + day;
                workSheet.Cells[row + 2, column++].Value = dateValue;
                var item = "";
                for(int col=0;col<headersId.Count;col++)
                {
                    var index = result.FindIndex(x => x.Date.ToString() == dateValue && x.UserId == headersId[col]);
                    if (index != -1)
                    {
                        workSheet.Cells[row + 2, column++].Value = result[index].Amount;
                        perUserTotalCost[col] += result[index].Amount;
                        item = result[index].Item;
                    }
                    else
                    {
                        workSheet.Cells[row + 2, column++].Value = 0;
                    }
                }
                workSheet.Cells[row + 2, column++].Value = item;
            }
             column = 1;
            workSheet.Cells[rowNumber + 2, column++].Value = "Remaining balance";
            for (int col = 0; col < headersId.Count; col++)
            {
                double value = 0;
                var index = perUserTotalAmount.FindIndex(x =>  x.UserId == headersId[col]);
                if (index != -1)
                {
                    value = perUserTotalAmount[index].Amount;

                }
                workSheet.Cells[rowNumber+2, column++].Value = value- perUserTotalCost[col];

            }
            

       

        }
    }
}
