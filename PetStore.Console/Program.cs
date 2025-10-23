using System;
using PetStore.Common.Models;
using PetStore.Common.Services;
using PetStore.Common.Extensions;

namespace PetStore.ConsoleApp
{
    internal class Program
    {
        static void Main()
        {
            System.Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("Ласкаво просимо до зоомагазину!\n");

            // створюємо CRUD сервіс для собак
            var dogService = new CrudService<Dog>();
            var dog1 = new Dog("Бобік", 3, "Лабрадор", true);
            var dog2 = new Dog("Макс", 2, "Хаскі", false);

            // підписка на подію гавкання
            dog1.OnBark += d => Console.WriteLine($"Собака {d.Name} привертає увагу клієнтів!");

            dogService.Create(dog1);
            dogService.Create(dog2);

            Console.WriteLine("Список собак:");
            dogService.ReadAll().ShowAllPets();

            dog1.Bark();

            // створюємо CRUD сервіс для котів
            var catService = new CrudService<Cat>();
            catService.Create(new Cat("Мурка", 1, "Сірий", true));
            catService.Create(new Cat("Сніжок", 4, "Білий", false));

            Console.WriteLine("\nСписок котів:");
            catService.ReadAll().ShowAllPets();

            // створюємо клієнтів
            var customer1 = new Customer("Олена", 25);
            var customer2 = new Customer("Іван", 30);

            customer1.ShowInfo();
            customer2.ShowInfo();
            Customer.ShowTotalCustomers();

            // Збереження у файл
            string filePath = "dogs.json";
            dogService.Save(filePath);
            Console.WriteLine($"\nДані збережено у файл {filePath}");

            // Очищаємо список і завантажуємо з файлу
            var newDogService = new CrudService<Dog>();
            newDogService.Load(filePath);
            Console.WriteLine("\nДані після завантаження з файлу:");
            newDogService.ReadAll().ShowAllPets();
        }
    }
}
