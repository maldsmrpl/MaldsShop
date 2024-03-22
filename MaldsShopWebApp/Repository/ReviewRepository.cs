using MaldsShopWebApp.Data;
using MaldsShopWebApp.Interfaces;
using MaldsShopWebApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaldsShopWebApp.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            return await SaveAsync();
        }

        public async Task<bool> UpdateAsync(Review review)
        {
            _context.Reviews.Update(review);
            return await SaveAsync();
        }

        public async Task<bool> DeleteAsync(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                return await SaveAsync();
            }
            return false;
        }

        public async Task<IEnumerable<Review>> GetAllByUserIdAsync(string userId)
        {
            return await _context.Reviews
                .Where(r => r.AppUserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetAllByUserEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                return await _context.Reviews
                    .Where(r => r.AppUserId == user.Id)
                    .ToListAsync();
            }
            return new List<Review>();
        }

        public async Task<IEnumerable<Review>> GetAllByProductIdAsync(int productId)
        {
            return await _context.Reviews
                .Where(r => r.ProductId == productId)
                .ToListAsync();
        }
        public async Task<Review> GetByIdAsync(int reviewId)
        {
            return await _context.Reviews.FirstOrDefaultAsync(i => i.ReviewId == reviewId);
        }

        private async Task<bool> SaveAsync()
        {
            return (await _context.SaveChangesAsync()) >= 0;
        }
    }
}
