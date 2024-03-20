using MaldsShopWebApp.Models;

namespace MaldsShopWebApp.Interfaces
{
    public interface IProductRepository
    {
        public Task<bool> AddAsync(Product product);
        public Task<bool> UpdateAsync(Product product);
        public Task<bool> DeleteAsync(Product product);
        public Task<bool> SaveAsync();
        public Task<IEnumerable<Product>> GetAllAsync();
        public Task<Product> GetByIdAsync(int id);
        public Task<PaginatedResult<Product>> GetAllPaginatedAsync(int pageIndex, int pageSize, string sortBy);
        public Task<bool> ProductSoldAsync(Product product, int itemsSold);
        public Task<bool> IsEnoughStock(Product product, int itemsRequired);
    }
}
