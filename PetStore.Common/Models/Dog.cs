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
    }
}
