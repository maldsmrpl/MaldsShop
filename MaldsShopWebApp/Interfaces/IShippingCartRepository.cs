namespace MaldsShopWebApp.Interfaces
{
    public interface IShippingCartRepository
    {
        Task<ShippingCart> GetShippingCartByUserEmail(string userEmail);
        Task<bool> AddToShippingCart(ShippingCartItem item, string userEmail);
        Task<bool> DeleteFromShippingCart(ShippingCartItem item, string userEmail);
        bool Save();
        Task<bool> SaveAsync();

    }
}
