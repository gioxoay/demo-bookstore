using AspNetCore.Identity.Mongo.Model;

namespace BookStore.Data.Domain
{
    public class User : MongoUser
    {
        public string DisplayName { get; set; }

        public string? AvatarUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; }
    }
}
