using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelFood.ViewModels
{
    public class DeleteDialogViewModel
    {
        public int CompanyId { get; set; }
        public int Id { get; set; }
        public string  Name { get; set; }

        public string ModelName { get; set; }

        public bool IsSupportDeactivation { get; set; }

        public string UserId { get; set; }
    }
}
