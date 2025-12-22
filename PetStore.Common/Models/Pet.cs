using System;

namespace PetStore.Common.Models
{
    // Базовий клас
    public abstract class Pet
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        // конструктор
        public Pet(string name, int age)
        {
            Id = Guid.NewGuid();
            Name = name;
            Age = age;
        }

        // метод
        public virtual void MakeSound()
        {
            Console.WriteLine($"{Name} видає звук");
        }
    }
}
