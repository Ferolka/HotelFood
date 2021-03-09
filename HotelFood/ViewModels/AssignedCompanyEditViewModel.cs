using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelFood.ViewModels
{
    public class AssignedCompanyEditViewModel
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public bool IsAssigned { get; set; }
        public bool IsCurrent { get; set; }
    }
}
