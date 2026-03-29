using System.ComponentModel.DataAnnotations;

namespace PetStore.Infrastructure.Models
{
    public class CatModel : PetModel
    {
        [Required]
        [MaxLength(50)]
        public string Color { get; set; } = string.Empty;

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

