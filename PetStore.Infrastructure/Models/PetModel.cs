using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PetStore.Infrastructure.Models
{
    [BsonDiscriminator(RootClass = true)]
    [BsonKnownTypes(typeof(DogModel), typeof(CatModel))]
    public abstract class PetModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("age")]
        public int Age { get; set; }

        [BsonElement("petType")]
        public string PetType { get; set; } = string.Empty;

        // One-to-One relationship: Each Pet has one Owner
        [BsonElement("ownerId")]
        [BsonRepresentation(BsonType.String)]
        public Guid? OwnerId { get; set; }

        protected PetModel()
        {
            Id = Guid.NewGuid();
        }

        protected PetModel(string name, int age, string petType)
        {
            Id = Guid.NewGuid();
            Name = name;
            Age = age;
            PetType = petType;
        }
    }
}

