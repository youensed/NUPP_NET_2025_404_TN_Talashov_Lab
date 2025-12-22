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

        // статичний метод для створення нового об'єкта із випадковими даними
        public static Cat CreateNew()
        {
            var random = new Random();
            var names = new[] { "Мурка", "Сніжок", "Тіша", "Мурзік", "Барсик", "Пушок", "Луна", "Сімба", "Персик", "Дімка" };
            var colors = new[] { "Сірий", "Білий", "Чорний", "Руді", "Смугастий", "Коричневий", "Плямистий" };
            
            var name = names[random.Next(names.Length)];
            var age = random.Next(1, 20);
            var color = colors[random.Next(colors.Length)];
            var isIndoor = random.Next(2) == 1;
            
            return new Cat(name, age, color, isIndoor);
        }
    }
}
