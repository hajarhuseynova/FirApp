using Fir.App.ViewModels;

namespace Fir.App.Services.Interfaces
{
    public interface IBasketService
    {
        public Task AddBasket(int id,int?count);
        public Task<List<BasketItemViewModel>> GetAllBaskets();
        public Task Remove(int id);
    }
}
