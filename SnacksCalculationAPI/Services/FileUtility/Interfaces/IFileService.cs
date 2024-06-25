using OfficeOpenXml;
using SnacksCalculationAPI.Services.FileUtility.Implementation;

namespace SnacksCalculationAPI.Services.FileUtility.Interfaces
{
    public interface IFileService
    {
        void SetTableStyle(ExcelWorksheet workSheet, int headersCount);
        void SetHeaderStyle(ExcelWorksheet workSheet, int headCount);
        void InsertHeaders(List<string> headers, ExcelWorksheet workSheet);
        FileData GetFileData(byte[] bytes);
        void AutoExcelFitColumns(int count, ExcelWorksheet workSheet);
    }
}
