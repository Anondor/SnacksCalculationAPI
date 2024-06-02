using SnacksCalculationAPI.Models;

namespace SnacksCalculationAPI.Services.AuthService
{
    public interface IAuthService
    {
        Task<object> GetJWTToken(LoginModel model,int userType);
    }
}
