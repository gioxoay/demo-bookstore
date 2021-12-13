using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace BookStore.Data.Domain
{
    public class Order
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string BookId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string StoreId { get; set; }

        public string BookName { get; set; }

        public string BookISBN { get; set; }

        public string ContactEmail { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}