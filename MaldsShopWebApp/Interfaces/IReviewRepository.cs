using MaldsShopWebApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaldsShopWebApp.Interfaces
{
    public interface IReviewRepository
    {
        Task<bool> AddAsync(Review review);
        Task<bool> UpdateAsync(Review review);
        Task<bool> DeleteAsync(int reviewId);
        Task<IEnumerable<Review>> GetAllByUserIdAsync(string userId);
        Task<IEnumerable<Review>> GetAllByUserEmailAsync(string email);
        Task<IEnumerable<Review>> GetAllByProductIdAsync(int productId);
        Task<Review> GetByIdAsync(int reviewId);
    }
}