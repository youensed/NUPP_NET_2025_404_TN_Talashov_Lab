using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PetStore.Common.Models;
using PetStore.Common.Services;

namespace PetStore.ConsoleApp
{
    internal class Program
    {
        static async Task Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Лабораторна робота №2 ===");
            Console.WriteLine("Багатопоковість. Асинхроність. IEnumerables. LINQ.\n");

            // Створюємо асинхроний CRUD сервіс
            var dogService = new CrudServiceAsync<Dog>("dogs_async.json");

            // Паралельне створення 1000+ об'єктів
            Console.WriteLine("Створення 1000 об'єктів Dog паралельно...\n");
            var stopwatch = Stopwatch.StartNew();
            
            Parallel.For(0, 1000, i =>
            {
                var dog = Dog.CreateNew();
                dogService.CreateAsync(dog).Wait();
            });
            
            stopwatch.Stop();
            Console.WriteLine($"Створено 1000 об'єктів за {stopwatch.ElapsedMilliseconds} мс\n");

            // Отримуємо всі об'єкти
            var allDogs = await dogService.ReadAllAsync();
            var dogsList = allDogs.ToList();

            // LINQ: Мінімальні, максимальні та середні значення
            Console.WriteLine("Статистика віку собак (LINQ):");
            Console.WriteLine($"   Мінімальний вік: {dogsList.Min(d => d.Age)} років");
            Console.WriteLine($"   Максимальний вік: {dogsList.Max(d => d.Age)} років");
            Console.WriteLine($"   Середній вік: {dogsList.Average(d => d.Age):F2} років");
            Console.WriteLine($"   Загальна кількість: {dogsList.Count} собак\n");

            // Статистика по породах
            var breedGroups = dogsList.GroupBy(d => d.Breed)
                .Select(g => new { Breed = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5);
            
            Console.WriteLine("Топ-5 порід:");
            foreach (var group in breedGroups)
            {
                Console.WriteLine($"   {group.Breed}: {group.Count} собак");
            }
            Console.WriteLine();

            // Збереження у файл
            Console.WriteLine("Збереження колекції у файл...");
            var saved = await dogService.SaveAsync();
            Console.WriteLine(saved ? "Дані успішно збережено у файл dogs_async.json\n" : "❌ Помилка збереження\n");

            // Пагінація
            Console.WriteLine("Приклад пагінації (перші 5 собак на сторінці 0):");
            var page0 = await dogService.ReadAllAsync(0, 5);
            foreach (var dog in page0)
            {
                Console.WriteLine($"   {dog.Name} ({dog.Breed}), {dog.Age} років");
            }
            Console.WriteLine();

            // Демонстрація IEnumerable
            Console.WriteLine("Використання IEnumerable (перші 3 собаки):");
            int count = 0;
            foreach (var dog in dogService)
            {
                if (count++ >= 3) break;
                Console.WriteLine($"   {dog.Name} - {dog.Breed}");
            }
            Console.WriteLine();

            // Демонстрація примітивів синхронізації
            await DemonstrateSynchronizationPrimitivesAsync();

            Console.WriteLine("Програма завершена!");
        }

        // Приклади використання примітивів синхронізації
        static async Task DemonstrateSynchronizationPrimitivesAsync()
        {
            Console.WriteLine("=== Демонстрація примітивів синхронізації ===\n");

            // 1. Lock - захист критичної секції
            DemonstrateLock();

            // 2. SemaphoreSlim - обмеження кількості одночасних операцій
            await DemonstrateSemaphoreAsync();

            // 3. AutoResetEvent - сигналізація між потоками
            DemonstrateAutoResetEvent();

            Console.WriteLine();
        }

        static void DemonstrateLock()
        {
            Console.WriteLine(" 1. Lock - захист спільного ресурсу:");
            var counter = 0;
            var lockObject = new object();

            Parallel.For(0, 100, i =>
            {
                lock (lockObject)
                {
                    counter++;
                }
            });

            Console.WriteLine($"   Лічильник після 100 паралельних інкрементів: {counter}");
            Console.WriteLine($"   (Без lock було б менше 100 через race condition)\n");
        }

        static async Task DemonstrateSemaphoreAsync()
        {
            Console.WriteLine(" 2. SemaphoreSlim - обмеження одночасного доступу:");
            var semaphore = new SemaphoreSlim(3, 3); // Максимум 3 одночасних операції
            var tasks = new Task[10];

            for (int i = 0; i < 10; i++)
            {
                var taskNumber = i;
                tasks[i] = Task.Run(async () =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        Console.WriteLine($"   Задача {taskNumber} виконується (макс 3 одночасно)");
                        await Task.Delay(100);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });
            }

            await Task.WhenAll(tasks);
            Console.WriteLine($"   Всі 10 задач виконано (по 3 одночасно)\n");
        }

        static void DemonstrateAutoResetEvent()
        {
            Console.WriteLine(" 3. AutoResetEvent - сигналізація між потоками:");
            var autoEvent = new AutoResetEvent(false);
            var workerFinished = false;

            // Робочий потік
            var workerThread = new Thread(() =>
            {
                Console.WriteLine("   Робочий потік: Виконую задачу...");
                Thread.Sleep(500);
                Console.WriteLine("   Робочий потік: Задачу виконано, сигнал надіслано!");
                workerFinished = true;
                autoEvent.Set(); // Надсилаємо сигнал
            });

            workerThread.Start();

            Console.WriteLine("   Головний потік: Очікую на завершення робочого потоку...");
            autoEvent.WaitOne(); // Чекаємо на сигнал
            Console.WriteLine($"   Головний потік: Отримано сигнал! Статус: {(workerFinished ? "Готово" : "Не готово")}\n");
        }
    }
}
