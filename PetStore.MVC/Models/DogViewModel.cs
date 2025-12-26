using System.ComponentModel.DataAnnotations;

namespace PetStore.MVC.Models
{
    public class DogViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Ім'я обов'язкове")]
        [StringLength(100, ErrorMessage = "Ім'я не може бути довшим за 100 символів")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вік обов'язковий")]
        [Range(0, 30, ErrorMessage = "Вік має бути від 0 до 30 років")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Порода обов'язкова")]
        [StringLength(100, ErrorMessage = "Порода не може бути довшою за 100 символів")]
        public string Breed { get; set; } = string.Empty;

        [Display(Name = "Дресирований")]
        public bool IsTrained { get; set; }
    }

    public class DogCreateViewModel
    {
        [Required(ErrorMessage = "Ім'я обов'язкове")]
        [StringLength(100, ErrorMessage = "Ім'я не може бути довшим за 100 символів")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вік обов'язковий")]
        [Range(0, 30, ErrorMessage = "Вік має бути від 0 до 30 років")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Порода обов'язкова")]
        [StringLength(100, ErrorMessage = "Порода не може бути довшою за 100 символів")]
        public string Breed { get; set; } = string.Empty;

        [Display(Name = "Дресирований")]
        public bool IsTrained { get; set; }
    }
}

