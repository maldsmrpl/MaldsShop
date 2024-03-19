namespace MaldsShopWebApp.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<AppUser>> GetAllUsers();
        Task<AppUser> GetUserById(string id);
        bool Add(AppUser user);
        bool Update(AppUser user);
        bool Delete(AppUser user);
        bool Save();
        bool IsAdminById(string userId);
        Task<bool> IsAdminByIdAsync(string userId);
        Task<AppUser> GetByEmail(string email);
        Task<bool> IsAdminByEmailAsync(string userEmail);
    }
}