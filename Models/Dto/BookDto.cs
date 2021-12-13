using System.ComponentModel.DataAnnotations;

namespace BookStore.Models.Dto
{
    [Display(Name = "Book")]
    public class BookDto
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public string ISBN { get; set; }

        public string Author { get; set; }

        public float MinPrice { get; set; }

        public float MaxPrice { get; set; }

        public int QuantityInStocks { get; set; }

        public IEnumerable<BookInStoreDto>? Stores { get; set; }
    }
}
