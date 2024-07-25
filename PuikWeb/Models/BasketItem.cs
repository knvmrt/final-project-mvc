namespace WebUI.Models
{
    public class BasketItem : BaseEntity
    {
        public Product Product { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public string AppUserId { get; set; }
        public virtual AppUser AppUser { get; set; }

    }

}
