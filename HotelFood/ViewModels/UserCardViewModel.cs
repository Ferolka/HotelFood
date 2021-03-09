using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelFood.ViewModels
{
    public class UserCardViewModel
    {
        public string UserId { get; set; }
        public string UserLogin { get; set; }

        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public string UserChildName { get; set; }
        public string CardToken { get; set; }

        public int? PictureId { get; set; }

    }
}
