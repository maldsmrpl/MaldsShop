using Microsoft.EntityFrameworkCore;
using MaldsShopWebApp.Interfaces;
using MaldsShopWebApp.Data;

namespace MaldsShopWebApp.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Add(AppUser user)
        {
            _context.Users.Add(user);
            return Save();
        }

        public bool Delete(AppUser user)
        {
            _context.Users.Remove(user);
            return Save();
        }

        public async Task<IEnumerable<AppUser>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<AppUser> GetUserById(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(AppUser user)
        {
            _context.Update(user);
            return Save();
        }
        public bool IsAdminById(string userId)
        {
            var adminRoleId = _context.Roles.FirstOrDefault(a => a.Name == UserRoles.Admin).Id;
            if (adminRoleId == null) return false;

            if (_context.UserRoles.FirstOrDefault(i => i.UserId == userId).RoleId == adminRoleId) 
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> IsAdminByIdAsync(string userEmail)
        {
            var adminRoleId = _context.Roles.FirstOrDefaultAsync(a => a.Name == UserRoles.Admin).Result.Id;
            if (adminRoleId == null)
            {
                Console.WriteLine($"ERROR: adminRoleId for \"{UserRoles.Admin}\" not found");
                return false;
            }
            
            var userId = _context.Users.FirstOrDefaultAsync(e => e.Email == userEmail).Result.Id.ToString();
            if (userId == null)
            {
                Console.WriteLine($"ERROR: userId with email:\"{userEmail}\" not found");
                return false;
            }

            if (_context.UserRoles.AnyAsync(ur => ur.RoleId == adminRoleId && ur.UserId == userId).Result)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> IsAdminByEmailAsync(string userEmail)
        {
            var userId = await _context.Users
                                       .Where(u => u.Email == userEmail)
                                       .Select(u => u.Id)
                                       .FirstOrDefaultAsync();

            if (userId == null)
            {
                return false;
            }

            var adminRoleId = await _context.Roles
                                            .Where(r => r.Name == UserRoles.Admin)
                                            .Select(r => r.Id)
                                            .FirstOrDefaultAsync();

            if (adminRoleId == null)
            {
                return false;
            }

            var isAdmin = await _context.UserRoles
                                         .AnyAsync(ur => ur.UserId == userId && ur.RoleId == adminRoleId);

            return isAdmin;
        }
        public async Task<AppUser> GetByEmailAsync(string email)
        {
            var user = await _context.Users
                .Include(s => s.ShippingCart)
                .ThenInclude(i => i.ShippingCartItems)
                .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(e => e.NormalizedEmail == email.ToUpper());

            if (user == null) return new AppUser();

            return user;
        }
        public async Task<AppUser> GetByEmailLazyAsync(string email)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(e => e.NormalizedEmail == email.ToUpper());

            if (user == null) return new AppUser();

            return user;
        }
        public async Task<bool> UpdateLastActivityAsync(string email)
        {
            var user = await GetByEmailLazyAsync(email);
            user.LastActivityTime = DateTime.UtcNow;
            return Update(user);
        }
    }
}