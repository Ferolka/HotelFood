using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace HotelFood.Models
{

    [Flags]
    public enum OrderTypeEnum
    {
        [Display(Name ="None")]
        None = 1,
        [Display(Name = "Complex")]
        Complex = 2,
        [Display(Name = "Dishes")]
        Dishes = 4,
        [Display(Name = "OneComplexType")]
        OneComplexType = 8
    }
    public class Hotel
    {
        public Hotel()
        {
            this.HotelGuests = new HashSet<HotelGuests>();
        }

        public int Id { get; set; }

        [StringLength(10)]
    
        public string Code { get; set; }
        [StringLength(40)]
      
        public string Name { get; set; }
        [DisplayName("Phone")]
        [StringLength(40)]
        public string Phone { get; set; }

        [StringLength(40)]
        public string Email { get; set; }

        [DisplayName("ZipCode")]
        [StringLength(10)]
        public string ZipCode { get; set; }
        [DisplayName("Country")]
        [StringLength(15)]
        public string Country { get; set; }
        [DisplayName("City")]
        [StringLength(25)]
        public string City { get; set; }
        [DisplayName("Address1")]
        [StringLength(40)]
        public string Address1 { get; set; }
        [DisplayName("Address2")]
        [StringLength(40)]
        public string Address2 { get; set; }
        [DisplayName("Logo")]
        public int? PictureId { get; set; }

        [DisplayName("StampLogo")]
        public int? StampPictureId { get; set; }
        [DisplayName("LeadTime")]
        public int? OrderLeadTimeH { get; set; }
        [DisplayName("ThresholdTime")]
        public int? OrderThresholdTimeH { get; set; }

        [DisplayName("IsDefault")]
        [DefaultValue(false)]
        public bool? IsDefault { get; set; }

        [DisplayName("OrderType")]
        [DefaultValue(0)]
        public int OrderType{ get; set; }

        public OrderTypeEnum GetOrderType()
        {
            return (OrderTypeEnum)this.OrderType;
        }
        public virtual ICollection<HotelGuests> HotelGuests { get; set; }

    }
}
