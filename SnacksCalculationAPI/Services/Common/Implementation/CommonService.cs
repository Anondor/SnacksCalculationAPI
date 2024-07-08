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
using Microsoft.AspNetCore.JsonPatch.Internal;

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

        private  Task<List<UserCostModel> > GetLastMonthData(string fromDate)
        {
            var lastMonthCostQuery =
                    from cm in _context.CostModels
            join um in _context.UserModels on cm.UserId equals um.Id
            where string.Compare(cm.Date, fromDate) < 0
            orderby cm.UserId ascending

            select new UserCostModel
            {
                Id = cm.Id,
                UserId = cm.UserId,
                Date = cm.Date,
                Amount = cm.Amount,
                Item = cm.Item,
            };
            var lastMonthCostResult =  lastMonthCostQuery.ToListAsync();
            return lastMonthCostResult;

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
        public async Task<FileData> GetMonthlyDetailsExcel(string fromDate, string toDate)
        {
            var lastMonthCostResult = await GetLastMonthData(fromDate);
            var thisMonthCostResult = await GetThisMonthData(fromDate, toDate);
            var userQuery = _context.UserModels.AsQueryable();
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
             Insert_ExcelRows(toDate,fromDate, headersId,  thisMonthCostResult,lastMonthCostResult, workSheet);
            _file.AutoExcelFitColumns(headers.Count, workSheet);

            FileData fileData = _file.GetFileData(excel.GetAsByteArray());
            excel.Dispose();
             return fileData;
        }
        private  List<double> GetReminingBalance(string fromDate, List<UserCostModel> lastMonthCostResult, List<int> headersId)
        {
            List<double> lastMonthReminingBalance = new List<double>();
            var perUserlastMonthAmount = _context.UserInformationModels
           .Where(x => string.Compare(x.Date, fromDate) < 0)
              .GroupBy(t => t.UserId)
              .Select(g => new UserCostData
              {
                  UserId = g.Key,
                  Amount = g.Sum(t => t.Amount)
              })
              .ToList();
            for (int index = 0; index < headersId.Count; index++)
            {
                var indexAmount = perUserlastMonthAmount.FindIndex(x => x.UserId == headersId[index]);
                var indexCost = lastMonthCostResult.FindIndex(x => x.UserId == headersId[index]);
                double balance = 0;
                if (indexAmount != -1)
                {
                    balance += perUserlastMonthAmount[indexAmount].Amount;
                }
                if (indexCost != -1)
                {
                    balance -= lastMonthCostResult[indexCost].Amount;
                }
                lastMonthReminingBalance.Insert(index, balance);
            }
           // List<double> lastMonthReminingBalance = lastMonthReminingBalance;

            return lastMonthReminingBalance;
        }

        private async Task<List<UserCostData>> GetThisMonthTotalAmount(string fromDate)
        {
            var perUserThisMonthAmount = _context.UserInformationModels
              .Where(x => string.Compare(x.Date, fromDate) >= 0)
                 .GroupBy(t => t.UserId)
                 .Select(g => new UserCostData
                 {
                     UserId = g.Key,
                     Amount = g.Sum(t => t.Amount)
                 })
                 .ToList();
            return perUserThisMonthAmount;
        }

        private async void Insert_ExcelRows(string toDate,string fromDate,List<int> headersId, List<UserCostModel> result, List<UserCostModel>lastMonthCostResult, ExcelWorksheet workSheet)
        {
            List<double> lastMonthReminingBalance =  GetReminingBalance(fromDate, lastMonthCostResult, headersId);
            var perUserThisMonthAmount = await GetThisMonthTotalAmount(fromDate);
            
            List<double> perUserTotalCost = new List<double>();
            for (int i = 0; i < headersId.Count; i++)
            {
                perUserTotalCost.Insert(i, 0);
            }

            int column;
            int row;
            var date = toDate.Substring(0, 8);
            var rowNumber = Int32.Parse(toDate.Substring(8, 2));
            for ( row=0;row< rowNumber; row++) {
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
             row = rowNumber + 3;
            workSheet.Cells[row , column++].Value = "Total Cost";
            for (int col=0;col<headersId.Count;col++)
            {
                workSheet.Cells[row, column++].Value = perUserTotalCost[col];
            }
            row++;
            column = 1;
            workSheet.Cells[row, column++].Value = "Total Balance";
            for (int col = 0; col < headersId.Count; col++)
            {
                double value1 = lastMonthReminingBalance[col];
                double value2 = 0;
                var index = perUserThisMonthAmount.FindIndex(x => x.UserId == headersId[col]);
                if (index != -1)
                {
                    value2 = perUserThisMonthAmount[index].Amount;
                }
                workSheet.Cells[row, column++].Value = value1 + value2;
            }

            row++;
            column = 1;
            workSheet.Cells[row , column++].Value = "Remaining balance";
            for (int col = 0; col < headersId.Count; col++)
            {
                double value = 0;
                var index = perUserThisMonthAmount.FindIndex(x =>  x.UserId == headersId[col]);
                if (index != -1)
                {
                    value = perUserThisMonthAmount[index].Amount;
                }
                workSheet.Cells[row , column++].Value = value- perUserTotalCost[col];
            }
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
            for(int index = 0;index<list.Count;index++)
            {
                int column = 1;
                workSheet.Cells[row , column++].Value = list[index].Date;
                workSheet.Cells[row , column++].Value = list[index].Amount;
                workSheet.Cells[row, column++].Value = list[index].Item;
                row++;
            }
            row++;
            workSheet.Cells[row, 1].Value = "Total Cost";
            workSheet.Cells[row, 2].Value = totalCost;
            row++;
            workSheet.Cells[row, 1].Value = "Total Amount";
            workSheet.Cells[row, 2].Value = totalAmount;
            row++;
            workSheet.Cells[row, 1].Value = "Remaining Balance";
            workSheet.Cells[row, 2].Value = totalAmount- totalCost;


        }
    }
}
