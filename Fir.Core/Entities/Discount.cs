using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fir.Core.Entities
{
    public class Discount:BaseModel
    {
        [Required]
        public double Percent { get; set; }
        public List<Product>? Products { get; set; }
    }
}
