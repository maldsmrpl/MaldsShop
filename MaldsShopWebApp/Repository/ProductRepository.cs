using MaldsShopWebApp.Interfaces;
using MaldsShopWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MaldsShopWebApp.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }
        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);

        }
        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);

        }
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }
        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products.Include(r => r.Reviews).ThenInclude(a => a.AppUser).AsNoTracking().FirstOrDefaultAsync(i => i.ProductId == id);
        }
        public async Task<PaginatedResult<Product>> GetAllPaginatedAsync(int pageIndex, int pageSize, string sortBy = "Name")
        {
            IQueryable<Product> query = _context.Products.Include(r => r.Reviews);

            switch (sortBy.ToLower())
            {
                case "name_asc":
                    query = query.OrderBy(p => p.Title);
                    break;
                case "name_desc":
                    query = query.OrderByDescending(p => p.Title);
                    break;
                case "price_asc":
                    query = query.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(p => p.Price);
                    break;
                // To be added case for reviews/scores
                default:
                    break;
            }

            var totalCount = await query.CountAsync();

            var items = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return new PaginatedResult<Product>
            {
                Items = items,
                TotalCount = totalCount
            };
        }
        public async Task ProductSoldAsync(Product product, int itemsSold)
        {
            if (await IsEnoughStock(product, itemsSold))
            {
                product.InStock -= itemsSold;
                product.ItemsSold += itemsSold;
                UpdateAsync(product);
            }
        }
        public async Task<bool> IsEnoughStock(Product product, int itemsRequired)
        {
            if (product == null || product.InStock < itemsRequired)
            {
                return false;
            }
            return true;
        }
    }
}
