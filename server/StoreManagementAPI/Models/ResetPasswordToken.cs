using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace StoreManagementAPI.Models
{
    public class ResetPasswordToken
    {
        [BsonId]
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = "";

        [BsonElement("userId")]
        public string UserId { get; set; } = "";

        [BsonElement("expiryDate")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime ExpiryDate { get; set; } = DateTime.Now;
    }
}
