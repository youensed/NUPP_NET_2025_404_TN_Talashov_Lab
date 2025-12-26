using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PetStore.Infrastructure.Models
{
    public class CustomerModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("age")]
        public int Age { get; set; }

        // One-to-Many relationship: Each Customer can have multiple Pets
        [BsonElement("petIds")]
        public List<Guid> PetIds { get; set; } = new List<Guid>();

        public CustomerModel()
        {
            Id = Guid.NewGuid();
        }

        public CustomerModel(string name, int age)
        {
            Id = Guid.NewGuid();
            Name = name;
            Age = age;
            PetIds = new List<Guid>();
        }
    }
}

