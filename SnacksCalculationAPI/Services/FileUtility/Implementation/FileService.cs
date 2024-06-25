using Microsoft.AspNetCore.StaticFiles;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SnacksCalculationAPI.Services.FileUtility.Interfaces;
using System.Drawing;

namespace SnacksCalculationAPI.Services.FileUtility.Implementation
{
    public class FileService : IFileService
    {
        public void SetTableStyle(ExcelWorksheet workSheet, int headersCount)
        {
            workSheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.TabColor = System.Drawing.Color.Black;
            workSheet.DefaultRowHeight = 12;
        }
        public void SetHeaderStyle(ExcelWorksheet workSheet, int headCount)
        {
            var header = workSheet.Cells[1, 1, 1, headCount];
            header.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            header.Style.Font.Bold = true;
            header.Style.Fill.PatternType = ExcelFillStyle.Solid;
            header.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(194, 194, 194));
            header.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            header.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            header.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            header.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        }
        public void InsertHeaders(List<string> headers, ExcelWorksheet workSheet)
        {
            for (int i = 0; i < headers.Count; i++)
            {
                workSheet.Cells[1, i + 1].Value = headers[i];
            }
        }
        public FileData GetFileData(byte[] bytes)
        {
            string fileName = Guid.NewGuid() + ".xlsx";
            string contentType;
            new FileExtensionContentTypeProvider().TryGetContentType(fileName, out contentType);
            FileData fileData = new FileData()
            {
                ContentType = contentType,
                Data = bytes,
                Name = fileName
            };
            return fileData;
        }
        public void AutoExcelFitColumns(int headerCount, ExcelWorksheet workSheet)
        {
            for (int i = 0; i < headerCount; i++)
            {
                workSheet.Column(i + 1).AutoFit();
            }
        }
    }
}
