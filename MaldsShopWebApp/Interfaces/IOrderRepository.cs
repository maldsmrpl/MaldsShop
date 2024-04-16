namespace MaldsShopWebApp.Interfaces
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(Order order);
        Task<Order> GetOrderByIdAsync(int id);
        void ConfirmPaymentAsync(Order order);
        Task<IEnumerable<Order>> GetOrdersByUserEmailLazyAsync(string email);
        Task<IEnumerable<Order>> GetOrdersByUserIdLazyAsync(string userId);
        Task<IEnumerable<Order>> GetOrdersByUserEmailAsync(string email);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
        Task<IEnumerable<Order>> GetOrdersByProductIdAsync(int productId);
        Task<Order> FindByStripeSessionIdAsync(string sessionId);
    }
}
