using Microsoft.AspNetCore.Identity;

namespace MaldsShopWebApp.Models
{
    public class AppUser : IdentityUser
    {
        DateTime AddedTime { get; set; }
        DateTime? LastActivityTime {  get; set; }
    }
}
