using Fir.App.ViewModels;

namespace Fir.App.Services.Interfaces
{
    public interface IBasketService
    {
        public Task AddBasket(int id);
        public Task<List<BasketItemViewModel>> GetAllBaskets();
        public Task Remove(int id);
    }
}
