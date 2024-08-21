using OfficeOpenXml.Style;
using OfficeOpenXml;
using SnacksCalculationAPI.Services.Common.Interfaces;
using SnacksCalculationAPI.Services.FileUtility.Implementation;
using SnacksCalculationAPI.Services.FileUtility.Interfaces;
using Microsoft.EntityFrameworkCore;
using SnacksCalculationAPI.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Drawing.Printing;
using System.Drawing;
using System;
using Microsoft.Extensions.Configuration;
using System.Xml.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Web;
using System.Reflection.PortableExecutable;

namespace SnacksCalculationAPI.Services.Common.Implementation
{
    public class CommonService :ICommonService
    {
        private readonly APIDbContext _context;
        private IFileService _file;
        public Microsoft.Extensions.Configuration.IConfiguration Configuration { get; }

        public CommonService(APIDbContext context, IFileService file, Microsoft.Extensions.Configuration.IConfiguration iConfiguration)
        {
            _context = context;
            _file = file;
            Configuration = iConfiguration;
        }
        private Task<List<UserCostModel>> GetThisMonthData(string fromDate,string toDate)
        {
            var thisMonthCostQuery = from cm in _context.CostModels
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
            var thisMonthCostResult =  thisMonthCostQuery.ToListAsync();
            return thisMonthCostResult;
        }

        private async Task<List<UserCostData>> GetThisTotalCostData(string fromDate, string toDate)
        {
            var perUserThisMonthCost = _context.CostModels
                .Where(x=>string.Compare(x.Date, fromDate) >= 0 && string.Compare(x.Date, toDate) <= 0)
                  .GroupBy(t => t.UserId)
                  .Select(g => new UserCostData
                  {
                      UserId = g.Key,
                      Amount = g.Sum(t => t.Amount)
                  })
                  .ToList();
            return perUserThisMonthCost;
        }
        private async Task<List<UserCostData>> GetTotalAmountData()
        {
            var perUserTotalAmount = _context.UserInformationModels 
                  .GroupBy(t => t.UserId)
                  .Select(g => new UserCostData
                  {
                      UserId = g.Key,
                      Amount = g.Sum(t => t.Amount)
                  })
                  .ToList();
            return perUserTotalAmount;
        }

        private async Task<List<UserCostData>> GetTotalCostData()
        {
            var perUserTotalCost = _context.CostModels
                  .GroupBy(t => t.UserId)
                  .Select(g => new UserCostData
                  {
                      UserId = g.Key,
                      Amount = g.Sum(t => t.Amount)
                  })
                  .ToList();
            return perUserTotalCost;
        }
        private async void Insert_ExcelRows_update(string toDate, string fromDate, List<int> headersId,List<UserCostData> perUserTotalAmount, List<UserCostData> perUserTotalCostAmount,
             List<UserCostModel> result, List<UserCostData> perUserThisMonthCost, ExcelWorksheet workSheet)
        {
            var perUserThisMonthAmount = await GetThisMonthTotalAmount(fromDate,toDate);

          

            int column;
            int row;
            var date = toDate.Substring(0, 8);
            var rowNumber = Int32.Parse(toDate.Substring(8, 2));
            for (row = 0; row < rowNumber; row++)
            {
                column = 1;
                var rowVal = (row + 1);
                var day = rowVal.ToString();
                if (day.Length == 1) day = "0" + day;
                var dateValue = date + day;
                workSheet.Cells[row + 2, column++].Value = dateValue;
                var item = "";
                for (int col = 0; col < headersId.Count; col++)
                {
                    var index = result.FindIndex(x => x.Date.ToString() == dateValue && x.UserId == headersId[col]);
                    if (index != -1)
                    {
                        workSheet.Cells[row + 2, column++].Value = result[index].Amount;
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
            row = rowNumber + 3;
            workSheet.Cells[row, column].Style.Font.Bold = true;
            workSheet.Cells[row, column++].Value = "Total Cost";
            for (int col = 0; col < headersId.Count; col++)
            {
                double cost = 0;
                var index= perUserThisMonthCost.FindIndex(x=>x.UserId == headersId[col]);
                if (index != -1)
                {
                    cost = perUserThisMonthCost[index].Amount;
                }
                workSheet.Cells[row, column++].Value = cost;
            }
            row++;
            column = 1;
            workSheet.Cells[row, column].Style.Font.Bold = true;
            workSheet.Cells[row, column++].Value = "Total Balance";
            for (int col = 0; col < headersId.Count; col++)
            {
                double balance = 0;
                var index1 = perUserTotalAmount.FindIndex(x => x.UserId == headersId[col]);
                if (index1 != -1)
                {
                    balance =balance+ perUserTotalAmount[index1].Amount;
                }
                var index2 = perUserTotalCostAmount.FindIndex(x => x.UserId == headersId[col]);
                if (index2 != -1)
                {
                    balance -= perUserTotalCostAmount[index2].Amount;
                }
                workSheet.Cells[row+1, column].Value = balance;

                var index3 = perUserThisMonthCost.FindIndex(x => x.UserId == headersId[col]);
                if (index3 != -1)
                {
                    balance += perUserThisMonthCost[index3].Amount;
                }
                workSheet.Cells[row, column++].Value = balance ;
            }
        
            
            row++;
            column = 1;
            workSheet.Cells[row, column].Style.Font.Bold = true;
            workSheet.Cells[row, column++].Value = "Remaining balance";
        }

        public async Task<FileData> GetMonthlyDetailsExcel(string fromDate, string toDate)
        {
            var perUserTotalAmount = await GetTotalAmountData();
            var perUserTotalCost = await GetTotalCostData();
            var perUserThisMonthCost = await GetThisTotalCostData(fromDate, toDate);
            var thisMonthCostResult = await GetThisMonthData(fromDate, toDate);

            var userQuery = _context.UserModels.AsQueryable();
            var userList = await userQuery.ToListAsync();
            ExcelPackage excel = new ExcelPackage();

            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
            var headers = new List<string>();
            var headersId = new List<int>();
            headers = ["Date"];
            for (int index = 0; index < userList.Count; index++)
            {
                headers.Insert(index + 1, userList[index].Name);
                headersId.Insert(index, userList[index].Id);
            }

            headers.Insert(userList.Count + 1, "Items");

            _file.SetTableStyle(workSheet, headers.Count);
            _file.SetHeaderStyle(workSheet, headers.Count);
            _file.InsertHeaders(headers, workSheet);
            Insert_ExcelRows_update(toDate, fromDate, headersId, perUserTotalAmount, perUserTotalCost, thisMonthCostResult, perUserThisMonthCost, workSheet);
            _file.AutoExcelFitColumns(headers.Count, workSheet);

            FileData fileData = _file.GetFileData(excel.GetAsByteArray());
            excel.Dispose();
            return fileData;

        }

        private async Task<List<UserCostData>> GetThisMonthTotalAmount(string fromDate,string toDate)
        {
            var perUserThisMonthAmount = _context.UserInformationModels
              .Where(x => string.Compare(x.Date, fromDate) >= 0 && string.Compare(x.Date, toDate)<=0)
                 .GroupBy(t => t.UserId)
                 .Select(g => new UserCostData
                 {
                     UserId = g.Key,
                     Amount = g.Sum(t => t.Amount)
                 })
                 .ToList();
            return perUserThisMonthAmount;
        }

        public async Task<FileData> GetExportGeneratedReportExcel(string fromDate, string toDate, int userId)
        {
            SqlConnection conn = null;
            SqlDataReader reader = null;
            try
            {
                var records = new List<CostModel>();
                double totalCost=0,totalAmount=0;

                var connStr = Configuration.GetConnectionString("DefaultConnection");
                 conn = new SqlConnection(connStr);
                conn.Open();
                var spName = "[dbo].[GetMonthlyUserCostData]";
                var command= new SqlCommand(spName, conn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 0 };

                command.Parameters.Add(new SqlParameter("@FromDate", SqlDbType.NVarChar) { Direction = ParameterDirection.Input, Value = fromDate });
                command.Parameters.Add(new SqlParameter("@ToDate", SqlDbType.NVarChar) { Direction = ParameterDirection.Input, Value = toDate });
                command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Direction = ParameterDirection.Input, Value = userId });

                reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    records.Add(new CostModel()
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        UserId = Convert.ToInt32(reader["UserId"]),
                        Date = HttpUtility.HtmlDecode(reader.GetString(reader.GetOrdinal("Date"))),
                        Amount = Convert.ToInt32(reader["Amount"]),
                        Item = HttpUtility.HtmlDecode(reader.GetString(reader.GetOrdinal("Item"))),

                    });
                }

                await reader.NextResultAsync();

                while (await reader.ReadAsync())
                {
                    totalCost = Convert.ToInt32(reader["Amount"]);
                }

                await reader.NextResultAsync();

                while (await reader.ReadAsync())
                {
                    totalAmount = Convert.ToInt32(reader["Amount"]);
                }
                var headers = new List<string>();
                headers = ["Date","Amount","Item"];
                ExcelPackage excel = new ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                _file.SetTableStyle(workSheet, headers.Count);
                _file.SetHeaderStyle(workSheet, headers.Count);
                _file.InsertHeaders(headers, workSheet);

                Insert_UserDataExcelRows(records, workSheet,totalCost,totalAmount);
                _file.AutoExcelFitColumns(headers.Count, workSheet);

                FileData fileData = _file.GetFileData(excel.GetAsByteArray());
                excel.Dispose();
                return fileData;
            }
            catch (Exception e)
            {
                throw e;
            }


        }

        private void Insert_UserDataExcelRows(List<CostModel> list, ExcelWorksheet workSheet, double totalCost, double totalAmount)
        {
            int row = 2;
            double totalMonthlyCost = 0.0;
            for(int index = 0;index<list.Count;index++)
            {
                int column = 1;
                workSheet.Cells[row , column++].Value = list[index].Date;
                workSheet.Cells[row , column++].Value = list[index].Amount;
                workSheet.Cells[row, column++].Value = list[index].Item;
                row++;
                totalMonthlyCost += list[index].Amount;
            }
            row++;
             workSheet.Cells[row,1].Style.Font.Bold = true;
            workSheet.Cells[row, 1].Value = "Total Cost";
           

            workSheet.Cells[row, 2].Value = totalMonthlyCost;
            row++;
            workSheet.Cells[row, 1].Style.Font.Bold = true;
            workSheet.Cells[row, 1].Value = "Total Amount";
            workSheet.Cells[row, 2].Value = totalAmount- totalCost+ totalMonthlyCost;
            row++;
            workSheet.Cells[row, 1].Style.Font.Bold = true;
            workSheet.Cells[row, 1].Value = "Remaining Balance";
            workSheet.Cells[row, 2].Value = totalAmount- totalCost;


        }
    }
}
