using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookStore.Data.Domain
{
    public class BookInStore
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string StoreId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string BookId { get; set; }

        public float Price { get; set; }

        public int Quantity { get; set; }
    }
}