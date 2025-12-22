using System;

namespace PetStore.Common.Models
{
    // клас-нащадок
    public class Dog : Pet
    {
        public string Breed { get; set; }
        public bool IsTrained { get; set; }

        // делегат і подія
        public delegate void BarkedHandler(Dog dog);
        public event BarkedHandler? OnBark;

        // конструктор
        public Dog(string name, int age, string breed, bool isTrained)
            : base(name, age)
        {
            Breed = breed;
            IsTrained = isTrained;
        }

        // метод
        public void Bark()
        {
            Console.WriteLine($"{Name} гавкає");
            OnBark?.Invoke(this);
        }

        // перевизначений метод
        public override void MakeSound()
        {
            Console.WriteLine($"{Name} гавкає гучно!");
        }

        // статичний метод для створення нового об'єкта із випадковими даними
        public static Dog CreateNew()
        {
            var random = new Random();
            var names = new[] { "Бобік", "Макс", "Рекс", "Чарлі", "Бадді", "Арчі", "Тоні", "Дюк", "Джек", "Рокі" };
            var breeds = new[] { "Лабрадор", "Хаскі", "Овчарка", "Бульдог", "Пудель", "Біглі", "Боксер", "Корги" };
            
            var name = names[random.Next(names.Length)];
            var age = random.Next(1, 15);
            var breed = breeds[random.Next(breeds.Length)];
            var isTrained = random.Next(2) == 1;
            
            return new Dog(name, age, breed, isTrained);
        }
    }
}
