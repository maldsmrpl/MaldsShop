namespace MaldsShopWebApp.Interfaces
{
    public interface IUnitOfWork
    {
        IOrderRepository Orders { get; }
        IProductRepository Products { get; }
        IReviewRepository Reviews { get; }
        IUserRepository Users { get; }
        IShippingCartRepository ShippingCarts { get; }
        Task<bool> CompleteAsync();
        void Dispose();
    }
}
