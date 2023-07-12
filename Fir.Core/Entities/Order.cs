using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fir.Core.Entities
{
    public class Order:BaseModel
    {
        public string AppUserId { get; set; }   
        public AppUser AppUser { get; set; }
        public decimal TotalPrice { get; set; } 
        public List<OrderItem> orderItems { get; set; } 
        public bool isAccepted { get; set; }    
        public bool isCompleted { get; set; }   
    }
}
