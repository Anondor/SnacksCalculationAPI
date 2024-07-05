using SnacksCalculationAPI.Core;
using SnacksCalculationAPI.Helpers;
using SnacksCalculationAPI.Extersions;

namespace SnacksCalculationAPI
{
    public static class AppIdentity
    {
        public static AppUserPrinciple AppUser
        {
            get
            {
                
                
                if (HttpHelper.HttpContext == null)
                {
                    return ClaimExtension.EmptyAppUser;
                }
                var user = HttpHelper.HttpContext.Items.ContainsKey("AppUser")
                    ? HttpHelper.HttpContext.Items["AppUser"] as AppUserPrinciple
                    : HttpHelper.HttpContext.User.ToAppUser();
                //var user = Thread.CurrentPrincipal as AppUserPrincipal;
                return user ?? ClaimExtension.EmptyAppUser;
            }
            set { ClaimExtension.EmptyAppUser = value; }

        }
    }
}
