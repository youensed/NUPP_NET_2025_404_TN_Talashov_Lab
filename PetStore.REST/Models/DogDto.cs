namespace PetStore.REST.Models
{
    public class DogDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Breed { get; set; } = string.Empty;
        public bool IsTrained { get; set; }
    }

    public class DogCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Breed { get; set; } = string.Empty;
        public bool IsTrained { get; set; }
    }

    public class DogUpdateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Breed { get; set; } = string.Empty;
        public bool IsTrained { get; set; }
    }
}


