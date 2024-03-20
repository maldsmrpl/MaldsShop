namespace MaldsShopWebApp.Interfaces
{
    public interface IOrderRepository
    {
        public Task<bool> AddAsync(Order order);
        public Task<bool> UpdateAsync(Order order);
        public Task<bool> DeleteAsync(Order order);
        public Task<bool> SaveAsync();
        public Task<Order> GetOrderByIdAsync(int id);
        public Task<bool> ConfirmPaymentAsync(Order order);
        public Task<IEnumerable<Order>> GetOrdersByUserEmailLazyAsync(string email);
        public Task<IEnumerable<Order>> GetOrdersByUserIdLazyAsync(string userId);
        public Task<IEnumerable<Order>> GetOrdersByUserEmailAsync(string email);
        public Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
        public Task<IEnumerable<Order>> GetOrdersByProductIdAsync(int productId);
        public Task<Order> FindByStripeSessionIdAsync(string sessionId);
    }
}
