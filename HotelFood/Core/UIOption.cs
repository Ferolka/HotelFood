using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelFood.Core
{
    public class UIOption
    {
        public UIOption()
        {
            CurrencySymbol = "₴";
            DefaultCulture = "uk-UA";
            DefaultRequestCulture = "uk";
        }
        public string CurrencySymbol { get; set; }

        public string DefaultCulture { get; set; }

        public string DefaultRequestCulture { get; set; }

    }
}
