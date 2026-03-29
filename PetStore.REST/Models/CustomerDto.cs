namespace PetStore.REST.Models
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public List<Guid> PetIds { get; set; } = new List<Guid>();
    }

    public class CustomerCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    public class CustomerUpdateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }
}

