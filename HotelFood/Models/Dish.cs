using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HotelFood.Models
{
    public class Dish: HotelDataOwnId
    {
        public Dish()
        {
            DishComplex = new HashSet<DishComplex>();
            
        
        }

        [StringLength(10)]
        [DataType(DataType.Text)]
        [DisplayName("Code")]
        //  [Required]
        public string Code { get; set; }

        [StringLength(100, MinimumLength = 2)]
     //   [RegularExpression("([a-zA-Z0-9 .&'-]+)", ErrorMessage = "The field Name should only include letters and number.")]
        [DataType(DataType.Text)]
        [Required]
        [DisplayName("Dish Name")]
        public string Name { get; set; }

        [Range(0, 1000)]
        [DataType(DataType.Currency)]
        [Required]
        [DisplayName("Dish Price")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }


       
    
  
        [DisplayName("Dish ReadyWeight")]
        [Column(TypeName = "decimal(18,3)")]
        public decimal ReadyWeight { get; set; }


        [DisplayName("Dish KKal")]
        public int KKal { get; set; }

        [StringLength(255)]
        [DataType(DataType.MultilineText)]
    //    [Required]
        [DisplayName("Dish Description")]
        public string Description { get; set; }

        [DisplayName("Dish Category")]
        public int CategoriesId { get; set; }

        [DisplayName("Cooking Technologie")]
        public string CookingTechnologie { get; set; }
        
        //Weight dish
       

        [DisplayName("Dish MeasureUnit")]
        public string? MeasureUnit { get; set; }
        //end weight dish

        
        [DisplayName("Dish Category")]
        public virtual Categories Category { get; set; }

      
        public int? PictureId { get; set; }
      


       

        public virtual ICollection<DishComplex> DishComplex { get; set; }
    }
}
