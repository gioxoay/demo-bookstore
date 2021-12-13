namespace BookStore.Models
{
    public class PlaceOrderRequest
    {
        public string ISBN { get; set; }

        public string StoreId { get; set; }

        public string ContactEmail { get; set; }
    }
}