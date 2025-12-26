namespace PetStore.Infrastructure.Models
{
    public class DogModel : PetModel
    {
        public string Breed { get; set; } = string.Empty;
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

