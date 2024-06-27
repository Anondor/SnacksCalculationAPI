using SnacksCalculationAPI.Core;
using System.Security.Claims;

namespace SnacksCalculationAPI.Extersions
{
    public static class ClaimExtension
    {
        
        public static AppUserPrinciple EmptyAppUser = new AppUserPrinciple("")
        {
            Id = 0,
            Name = string.Empty,
            Email = string.Empty,
            UserAgentInfo = "CL:en-US|IP:127.0.0.1",
        };
    
            private static AppUserPrinciple AppUser(Dictionary<string, string> claims)
        {
            if (claims.Count == 0)
            {
                return EmptyAppUser;
            }
            return new AppUserPrinciple(claims["UserName"])
            {
                Id = Convert.ToInt32(claims["Id"]),
                Name = claims["Name"],
                Phone = claims["Phone"],
                Email = claims["Email"],
                Avatar = claims["Avatar"],
                ActiveRoleId = Convert.ToInt32(claims["ActiveRoleId"]),
                RoleIds = claims["RoleIds"],
                UserAgentInfo = claims["UserAgentInfo"],
                ActiveRoleName = claims["ActiveRoleName"],
                NodeId = Convert.ToInt32(claims["NodeId"])
            };
        }
        public static AppUserPrinciple ToAppUser(this ClaimsPrincipal user)
        {
            IEnumerable<Claim> userClaimes;
            if (user != null)
            {
                userClaimes = user.Claims;
            }
            else
            {
                if (user == null && user.Identity != null)
                {
                    userClaimes = ((ClaimsIdentity)user.Identity).Claims;
                }
                else
                {
                    return EmptyAppUser;
                }
            }

            return userClaimes.ToAppUser();

        }

        public static AppUserPrinciple ToAppUser(this IEnumerable<Claim> claims)
        {
            if (claims == null)
            {
                return EmptyAppUser;
            }
            var claimItems = claims
               .Where(s => !s.Type.Contains("schemas.microsoft.com") && !s.Type.Contains("schemas.xmlsoap.org"))
               .Select(x => new { Key = x.Type, x.Value })
               .ToDictionary(t => t.Key, t => t.Value);

            return AppUser(claimItems) ?? EmptyAppUser;
        }
    }
}
