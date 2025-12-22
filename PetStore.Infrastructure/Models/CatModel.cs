using MongoDB.Bson.Serialization.Attributes;

namespace PetStore.Infrastructure.Models
{
    [BsonDiscriminator("Cat")]
    public class CatModel : PetModel
    {
        [BsonElement("color")]
        public string Color { get; set; } = string.Empty;

        [BsonElement("isIndoor")]
        public bool IsIndoor { get; set; }

        public CatModel() : base()
        {
            PetType = "Cat";
        }

        public CatModel(string name, int age, string color, bool isIndoor)
            : base(name, age, "Cat")
        {
            Color = color;
            IsIndoor = isIndoor;
        }
    }
}

