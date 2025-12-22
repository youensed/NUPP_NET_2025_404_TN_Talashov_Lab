using MongoDB.Bson.Serialization.Attributes;

namespace PetStore.Infrastructure.Models
{
    [BsonDiscriminator("Dog")]
    public class DogModel : PetModel
    {
        [BsonElement("breed")]
        public string Breed { get; set; } = string.Empty;

        [BsonElement("isTrained")]
        public bool IsTrained { get; set; }

        public DogModel() : base()
        {
            PetType = "Dog";
        }

        public DogModel(string name, int age, string breed, bool isTrained)
            : base(name, age, "Dog")
        {
            Breed = breed;
            IsTrained = isTrained;
        }
    }
}

