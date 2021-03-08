using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace HotelFood.Models
{

   
    public class HotelUser :IdentityUser<string>
    {
        public HotelUser()
        {
            this.RoomGuests = new HashSet<HotelUser>();
        }
        public int HotelId { get; set; }

        [StringLength(10)]
        public string ZipCode { get; set; }

        [StringLength(15)]
        public string Country { get; set; }
        [StringLength(25)]
        public string City { get; set; }

        [StringLength(40)]
        public string Address1 { get; set; }

     

        [StringLength(40)]
        public string NameSurname { get; set; }


        [DefaultValue(1)]
        public int GuestCount { get; set; }

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        [DisplayName("User Group")]
        public int? UserGroupId { get; set; }

        [DisplayName("User Group")]
        public virtual UserGroups UserGroup { get; set; }

        public int?  MenuType { get; set; }

        [DefaultValue(false)]
        public bool ConfirmedByAdmin { get; set; }

        public virtual ICollection<HotelUser> RoomGuests { get; set; }

        

        [StringLength(64)]
        public string CardTag { get; set; }
        public virtual ICollection<HotelGuests> HotelGuests  { get; set; }
    }
    public class HotelRole : IdentityRole
    {

       // public int CompanyId { get; set; }

    }
}
