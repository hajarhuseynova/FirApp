using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fir.Core.Entities
{
    public class OrderItem:BaseModel
    {
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public Product Product { get; set; }
        public Order Order { get; set; }
        public int ProductCount { get; set; }   

    }
}
