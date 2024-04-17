using MaldsShopWebApp.Models;

namespace MaldsShopWebApp.Interfaces
{
    public interface IProductRepository
    {
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task<PaginatedResult<Product>> GetAllPaginatedAsync(int pageIndex, int pageSize, string sortBy);
        Task<bool> ProductSoldAsync(Product product, int itemsSold);
        Task<bool> IsEnoughStock(Product product, int itemsRequired);
    }
}
