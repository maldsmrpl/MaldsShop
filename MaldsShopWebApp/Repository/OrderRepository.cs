﻿using MaldsShopWebApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MaldsShopWebApp.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }
        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
        }
        public async Task DeleteAsync(Order order)
        {
            _context.Orders.Remove(order);
        }
        public async Task<Order> GetOrderByIdAsync(int id)
        {
            Order order = await _context.Orders
                .Include(i => i.OrderItems)
                .ThenInclude(p => p.Product)
                .Include(u => u.AppUser)
                .FirstOrDefaultAsync(i => i.OrderId == id);

            return order;
        }
        public void ConfirmPaymentAsync(Order order)
        {
            if (order == null || order.IsPaid)
            {
                throw new InvalidOperationException("Order is already paid or null.");
            }

            order.IsPaid = true;
            UpdateAsync(order);
        }
        public async Task<IEnumerable<Order>> GetOrdersByUserEmailLazyAsync(string email)
        {
            return await _context.Orders
                .Where(e => e.AppUser.Email == email)
                .ToListAsync();
        }
        public async Task<IEnumerable<Order>> GetOrdersByUserIdLazyAsync(string userId)
        {
            return await _context.Orders
                .Where(i => i.AppUserId == userId)
                .ToListAsync();
        }
        public async Task<IEnumerable<Order>> GetOrdersByUserEmailAsync(string email)
        {
            return await _context.Orders
                .Include(u => u.AppUser)
                .Include(i => i.OrderItems)
                .ThenInclude(p => p.Product)
                .Where(e => e.AppUser.Email == email)
                .OrderByDescending(o => o.PurchasedTime)
                .ToListAsync();
        }
        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _context.Orders
                .Where(i => i.AppUserId == userId)
                .ToListAsync();
        }
        public async Task<IEnumerable<Order>> GetOrdersByProductIdAsync(int productId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.AppUser)
                .Where(o => o.OrderItems
                .Any(oi => oi.ProductId == productId))
                .ToListAsync();
        }
        public async Task<Order> FindByStripeSessionIdAsync(string sessionId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.StripeSessionId == sessionId);
        }
    }
}
