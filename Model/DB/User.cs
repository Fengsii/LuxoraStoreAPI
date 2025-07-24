using System.Net;
using static LuxoraStore.Model.GeneralStatus.GeneralStatusData;

namespace LuxoraStore.Model.DB
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // Store hashed password
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public string Role { get; set; } // "SuperAdmin", "Customer", "Seller"
        public GeneralStatusDataAll UserStatus { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

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
