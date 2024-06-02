using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SnacksCalculationAPI.Core;
using SnacksCalculationAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SnacksCalculationAPI.Services.AuthService
{
    public class AuthService :IAuthService
    {
        private readonly IConfiguration configuration;
        private readonly APIDbContext _context;

        public AuthService(IConfiguration config, APIDbContext context)
        {
            configuration = config;
            _context = context;
        }
        public async Task<object> GetJWTToken(LoginModel model,int userType)
        {
            try
            {
                var userInfo = await _context.UserModels.FirstOrDefaultAsync(x => x.Phone == model.Phone && x.Password == model.Password && x.UserType== userType);
                if (userInfo == null)
                    throw new Exception("Invalid HHT User");

                var user = new AppUserPrinciple("brainstation23")
                {
                    Id = userInfo.Id,
                    Name = userInfo.Name,
                    RoleIdList = new List<int> { 0 },
                    Avatar = "/img/user.png",
                    Phone = userInfo.Phone,
                    Email = userInfo.Email,
                    UserAgentInfo = "127.0.0.1",

                };
                var appClaimes = user
                                .GetByName()
                                .Select(item => new Claim(item.Key, item.Value));

                var claims = new List<Claim>()
                    {

                            new Claim(JwtRegisteredClaimNames.UniqueName,user.Name),
                            new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
                            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    };
                claims.AddRange(appClaimes);
                foreach (var role in user.RoleIdList)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Tokens:key"]));
                var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    configuration["Tokens:Issuer"],
                    configuration["Tokens:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddHours(100.00),
                    signingCredentials: cred
                    );
                var results = new
                {
                    Id = userInfo.Id,
                    Name = userInfo.Name,
                    Phone = userInfo.Phone,
                    Email = userInfo.Email,
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,

                };

                return results;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

    }
}
