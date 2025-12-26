using System.ComponentModel.DataAnnotations;

namespace PetStore.MVC.Models
{
    public class CatViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Ім'я обов'язкове")]
        [StringLength(100, ErrorMessage = "Ім'я не може бути довшим за 100 символів")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вік обов'язковий")]
        [Range(0, 25, ErrorMessage = "Вік має бути від 0 до 25 років")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Колір обов'язковий")]
        [StringLength(50, ErrorMessage = "Колір не може бути довшим за 50 символів")]
        public string Color { get; set; } = string.Empty;

        [Display(Name = "Домашній")]
        public bool IsIndoor { get; set; }
    }

    public class CatCreateViewModel
    {
        [Required(ErrorMessage = "Ім'я обов'язкове")]
        [StringLength(100, ErrorMessage = "Ім'я не може бути довшим за 100 символів")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вік обов'язковий")]
        [Range(0, 25, ErrorMessage = "Вік має бути від 0 до 25 років")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Колір обов'язковий")]
        [StringLength(50, ErrorMessage = "Колір не може бути довшим за 50 символів")]
        public string Color { get; set; } = string.Empty;

        [Display(Name = "Домашній")]
        public bool IsIndoor { get; set; }
    }
}

