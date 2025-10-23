using System;

namespace PetStore.Common.Models
{
    public class Cat : Pet
    {
        public string Color { get; set; }
        public bool IsIndoor { get; set; }

        // конструктор
        public Cat(string name, int age, string color, bool isIndoor)
            : base(name, age)
        {
            Color = color;
            IsIndoor = isIndoor;
        }

        // метод
        public void Purr()
        {
            Console.WriteLine($"{Name} муркоче");
        }

        public override void MakeSound()
        {
            Console.WriteLine($"{Name} нявкає!");
        }
    }
}
