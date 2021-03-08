using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HotelFood.Models
{
    public class Pictures
    {
        public int Id { get; set; }


        [Column(TypeName = "image")]
        public byte[] PictureData { get; set; }
    }
}
