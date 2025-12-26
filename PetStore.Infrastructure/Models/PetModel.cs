namespace PetStore.Infrastructure.Models
{
    public abstract class PetModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string PetType { get; set; } = string.Empty;

        // One-to-Many relationship: Each Pet belongs to one Customer
        public int? OwnerId { get; set; }
        public virtual CustomerModel? Owner { get; set; }

        // Many-to-Many relationship: Each Pet can have multiple Vaccines
        public virtual ICollection<VaccineModel> Vaccines { get; set; } = new List<VaccineModel>();

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

