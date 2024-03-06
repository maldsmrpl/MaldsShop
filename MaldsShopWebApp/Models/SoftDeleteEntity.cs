namespace MaldsShopWebApp.Models
{
    public abstract class SoftDeleteEntity
    {
        public bool IsDeleted { get; set; }
    }
}