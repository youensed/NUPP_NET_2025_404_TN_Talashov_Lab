using System.ComponentModel.DataAnnotations;

namespace PetStore.Infrastructure.Models
{
    public class CustomerModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public int Age { get; set; }

        // One-to-Many relationship: Each Customer can have multiple Pets
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

