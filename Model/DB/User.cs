using System.Net;
using static LuxoraStore.Model.GeneralStatusData;

namespace LuxoraStore.Model.DB
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // "Admin", "Customer",
        public GeneralStatusDataAll UserStatus { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // OTP Fields
        public string? OtpCode { get; set; }
        public DateTime? OtpExpiredAt { get; set; }

        // Navigation properties
        //public SellerProfile SellerProfile { get; set; }
        //public Wallet Wallet { get; set; }
        //public ICollection<Address> Addresses { get; set; }
        //public ICollection<Cart> Carts { get; set; }
        //public ICollection<Order> Orders { get; set; }
        //public ICollection<ProductReview> ProductReviews { get; set; }
        //public ICollection<ProductLike> ProductLikes { get; set; }

    }
}
