using System;

namespace PetStore.Common.Models
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        // статичне поле
        public static int TotalCustomers;

        // статичний конструктор
        static Customer()
        {
            TotalCustomers = 0;
        }

        // конструктор
        public Customer(string name, int age)
        {
            Id = Guid.NewGuid();
            Name = name;
            Age = age;
            TotalCustomers++;
        }

        // метод
        public void ShowInfo()
        {
            Console.WriteLine($"{Name}, {Age} років (Загальна кількість клієнтів: {TotalCustomers})");
        }

        // статичний метод
        public static void ShowTotalCustomers()
        {
            Console.WriteLine($"Зареєстровано клієнтів: {TotalCustomers}");
        }
    }
}
