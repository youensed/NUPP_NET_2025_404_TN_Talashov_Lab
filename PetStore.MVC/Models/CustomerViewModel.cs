using System.ComponentModel.DataAnnotations;

namespace PetStore.MVC.Models
{
    public class CustomerViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Ім'я обов'язкове")]
        [StringLength(100, ErrorMessage = "Ім'я не може бути довшим за 100 символів")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вік обов'язковий")]
        [Range(18, 120, ErrorMessage = "Вік має бути від 18 до 120 років")]
        public int Age { get; set; }
    }

    public class CustomerCreateViewModel
    {
        [Required(ErrorMessage = "Ім'я обов'язкове")]
        [StringLength(100, ErrorMessage = "Ім'я не може бути довшим за 100 символів")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вік обов'язковий")]
        [Range(18, 120, ErrorMessage = "Вік має бути від 18 до 120 років")]
        public int Age { get; set; }
    }
}

