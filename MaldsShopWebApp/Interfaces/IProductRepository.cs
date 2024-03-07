namespace MaldsShopWebApp.Interfaces
{
    public interface IProductRepository
    {
        public bool Add(Product product);
        public bool Update(Product product);
        public bool Delete(Product product);
        public bool Save(Product product);
        public IEnumerable<Product> GetAll();
        public Task<IEnumerable<Product>> GetAllAsync();
    }
}
