using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HotelFood.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace HotelFood.Core
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<HotelUser, HotelRole>
    {
        public CustomClaimsPrincipalFactory(UserManager<HotelUser> userManager,
                                                RoleManager<HotelRole> roleManager,
                                                IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
        {
        }

        public async override Task<ClaimsPrincipal> CreateAsync(HotelUser user)
        {
            var principal = await base.CreateAsync(user);

            // Add your claims here
            ((ClaimsIdentity)principal.Identity).AddClaims(
                new[] { new Claim("hotelid", user.HotelId.ToString())           
               });

            return principal;
        }

        public static ClaimsPrincipal ChangeCompanyId(ClaimsPrincipal claims,int companyId)
        {
            var identity = claims.Identity as ClaimsIdentity;
            var claim = claims.FindFirst("hotelid");
            if (claim != null)
                identity.RemoveClaim(claim);
            identity.AddClaim(new Claim("hotelid", companyId.ToString()));
            return claims;
        }
    }
}
