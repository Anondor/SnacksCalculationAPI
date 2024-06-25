using SnacksCalculationAPI.Services.FileUtility.Implementation;

namespace SnacksCalculationAPI.Services.Common.Interfaces
{
    public interface ICommonService
    {
        Task<FileData> GetMonthlyDetailsExcel(string fromDate, string toDate);
    }
}
