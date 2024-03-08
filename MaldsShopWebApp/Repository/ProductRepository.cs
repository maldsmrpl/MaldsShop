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
        public bool Add(Product product)
        {
            _context.Products.Add(product);
            return Save();
        }
        public bool Update(Product product)
        {
            _context.Products.Update(product);
            return Save();
        }
        public bool Delete(Product product)
        {
            _context.Products.Remove(product);
            return Save();
        }
        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }
        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products.AsNoTracking().FirstOrDefaultAsync(i => i.ProductId == id);
        }
        public async Task<PaginatedResult<Product>> GetAllPaginatedAsync(int pageIndex, int pageSize, string sortBy = "Name")
        {
            IQueryable<Product> query = _context.Products;

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
    }
}
