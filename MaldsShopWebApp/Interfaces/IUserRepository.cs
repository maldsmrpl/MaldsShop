namespace MaldsShopWebApp.Interfaces
{
    public interface IUserRepository
    {
        Task AddAsync(AppUser user);
        void Update(AppUser user);
        void Delete(AppUser user);
        Task<IEnumerable<AppUser>> GetAllUsers();
        Task<AppUser> GetUserById(string id);
        Task<bool> IsAdminByIdAsync(string userId);
        Task<bool> IsAdminByEmailAsync(string userEmail);
        Task<AppUser> GetByEmailAsync(string email);
        Task<AppUser> GetByEmailLazyAsync(string email);
        Task UpdateLastActivityAsync(string email);
	}
}