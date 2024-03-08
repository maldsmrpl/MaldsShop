using MaldsShopWebApp.Models;

namespace MaldsShopWebApp.Interfaces
{
    public interface IProductRepository
    {
        public bool Add(Product product);
        public bool Update(Product product);
        public bool Delete(Product product);
        public bool Save();
        public Task<IEnumerable<Product>> GetAllAsync();
        public Task<Product> GetByIdAsync(int id);
        Task<PaginatedResult<Product>> GetAllPaginatedAsync(int pageIndex, int pageSize, string sortBy);
    }
}
