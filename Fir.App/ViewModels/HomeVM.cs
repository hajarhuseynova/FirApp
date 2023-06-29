using Fir.Core.Entities;

namespace Fir.App.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Category> categories { get; set; }
        public IEnumerable<Blog> blogs { get; set; }
    }
}
