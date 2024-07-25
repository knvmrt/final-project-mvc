namespace WebUI.Models
{
    public class WishListItem : BaseEntity
    {
        public Product Product { get; set; }
        public int ProductId { get; set; }
        public string AppUserId { get; set; }
        public virtual AppUser AppUser { get; set; }
    }
}
