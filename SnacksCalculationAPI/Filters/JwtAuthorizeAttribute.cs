using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace SnacksCalculationAPI.Filters
{
    public class JwtAuthorizeAttribute : AuthorizeAttribute
    {
        public JwtAuthorizeAttribute()
        {
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
        }

        public JwtAuthorizeAttribute(string policy) : base(policy)
        {
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;

        }
    }
 }
