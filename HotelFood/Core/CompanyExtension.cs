using HotelFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelFood.Core
{
    public static class CompanyExtension
    {
        
        public static IEnumerable<string> EMails(this IEnumerable<HotelUser> src)
        {
            return src.Select(u => u.Email);
        }
    }
}
