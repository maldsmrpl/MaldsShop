namespace MaldsShopWebApp.Interfaces
{
    public interface IShippingCartRepository
    {
        Task AddAsync(ShippingCart shippingCart);
        Task UpdateAsync(ShippingCart shippingCart);
        Task AddToShippingCartAsync(ShippingCartItem item, string userEmail);
        Task DeleteFromShippingCart(ShippingCartItem item, string userEmail);
        Task ClearShippingCart(ShippingCart shippingCart);
        Task<ShippingCart>? GetShippingCartByUserEmail(string userEmail);
    }
}
