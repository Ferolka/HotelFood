using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HotelFood.Models
{
    public class UserGroups: HotelDataOwnId
    {
        public UserGroups()
        {
            HotelUsers = new  HashSet<HotelUser>();
        }


        [StringLength(100, MinimumLength = 2)]
        [DataType(DataType.Text)]
        [Required]
        [DisplayName("MenuUserGroups")]
        public string Name { get; set; }


        [DisplayName("User Group")]
        public virtual UserGroups UserGroup { get; set; }

        public virtual ICollection<HotelUser> HotelUsers { get; set; }
    }
}