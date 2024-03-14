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
        Task<IEnumerable<Review>> GetAllByUserId(string userId);
        Task<IEnumerable<Review>> GetAllByUserEmail(string email);
        Task<IEnumerable<Review>> GetAllByProductId(int productId);
        Task<Review> GetById(int reviewId);
    }
}