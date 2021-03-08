using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace HotelFood.Core
{
    public static class UserExtension
    {
        public const string UserRole_Admin = "Admin";
        public const string UserRole_CompanyAdmin = "CompanyAdmin";
        public const string UserRole_GroupAdmin = "GroupAdmin";
        public const string UserRole_UserAdmin = "UserAdmin";
        public const string UserRole_KitchenAdmin = "KitchenAdmin";
        public const string UserRole_SubGroupAdmin = "SubGroupAdmin";
        public const string UserRole_ServiceAdmin = "ServiceAdmin";
        public const string UserRole_SubGroupReportAdmin = "SubGroupReportAdmin";
        public static string GetUserId(this IPrincipal principal)
        {
            var claimsIdentity = (ClaimsIdentity)principal.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return claim.Value;
        }
        public static int GetHotelID(this IPrincipal principal)
        {
            var claimsIdentity = (ClaimsIdentity)principal.Identity;
            var claim = claimsIdentity.FindFirst("hotelid");
            if (claim == null)
                return 0;// claim.Value;
            return int.Parse(claim.Value);
        }
        
    }
}
