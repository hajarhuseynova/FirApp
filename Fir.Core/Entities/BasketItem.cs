using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fir.Core.Entities
{
    public class BasketItem:BaseModel
    {
        public int ProductId { get; set; }
        public int BasketId { get; set; }
        public Basket Basket { get; set; }  
        public Product Product { get; set; }
        public int ProductCount { get; set; }

    }
}
