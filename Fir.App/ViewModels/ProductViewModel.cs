using Fir.Core.Entities;

namespace Fir.App.ViewModels
{
    public class ProductViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public Product Product { get; set; }
    }
}
