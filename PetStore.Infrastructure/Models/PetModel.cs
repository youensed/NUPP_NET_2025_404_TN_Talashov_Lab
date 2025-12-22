using System.ComponentModel.DataAnnotations;

namespace PetStore.Infrastructure.Models
{
    public abstract class PetModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public int Age { get; set; }

        [Required]
        [MaxLength(50)]
        public string PetType { get; set; } = string.Empty;

        // One-to-One relationship: Each Pet has one Owner
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

