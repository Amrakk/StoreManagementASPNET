using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson;

namespace StoreManagementAPI.Models
{
    public class Order
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Oid { get; set; } = "";

        [BsonElement("customer")]
        public Customer Customer { get; set; } = new Customer();

        [BsonElement("user")]
        public User User { get; set; } = new User();

        [BsonElement("totalPrice")]
        [BsonRepresentation(BsonType.Double)]
        public double TotalPrice { get; set; } = 0;

        [BsonElement("orderStatus")]
        [BsonRepresentation(BsonType.String)]
        public Status OrderStatus { get; set; } = Status.PENDING;

        [BsonElement("orderProducts")]
        public List<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

        [BsonElement("createdAt")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        [BsonElement("updatedAt")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime? UpdatedAt { get; set; } = null;


        [BsonElement("_class")]
        public string _class { get; set; } = "";


        public override string ToString()
        {
            return $"[Order] Oid: {Oid}, Customer: {Customer}, User: {User}, TotalPrice: {TotalPrice}, OrderStatus: {OrderStatus}, OrderProducts: {OrderProducts}, CreatedAt: {CreatedAt}, UpdatedAt: {UpdatedAt}, _class: {_class}";
        }

    }
}
