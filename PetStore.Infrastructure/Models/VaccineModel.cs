namespace PetStore.Infrastructure.Models
{
    public class VaccineModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Many-to-Many relationship: Each Vaccine can be given to multiple Pets
        public virtual ICollection<PetModel> Pets { get; set; } = new List<PetModel>();

        public VaccineModel()
        {
        }

        public VaccineModel(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}

