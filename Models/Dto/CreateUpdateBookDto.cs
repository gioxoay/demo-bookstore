using System.ComponentModel.DataAnnotations;

namespace BookStore.Models.Dto
{
    public class CreateUpdateBookDto
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        public string ISBN { get; set; }

        public string Author { get; set; }

        public float MinPrice { get; set; }

        public float MaxPrice { get; set; }

        public int Stocks { get; set; }

        public string StoreId { get; set; }
    }
}
