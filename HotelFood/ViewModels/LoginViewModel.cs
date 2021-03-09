using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelFood.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [DisplayName("User Name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("User Password")]
        public string Password { get; set; }

        [ScaffoldColumn(false)]
        public string ReturnUrl { get; set; }

        public bool IsModal { get; set; }

        [DisplayName("RemeberMe")]
        public bool IsRemember { get; set; }

    }
}
