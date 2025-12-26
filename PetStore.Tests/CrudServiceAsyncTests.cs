using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PetStore.Common.Models;
using PetStore.Common.Services;
using Xunit;

namespace PetStore.Tests
{
    public class CrudServiceAsyncTests : IDisposable
    {
        private readonly string _testFilePath;

        public CrudServiceAsyncTests()
        {
            _testFilePath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.json");
        }

        public void Dispose()
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }

        [Fact]
        public async Task CreateAsync_ShouldAddElement()
        {
            // Arrange
            var service = new CrudServiceAsync<Dog>(_testFilePath);
            var dog = new Dog("Тест", 5, "Лабрадор", true);

            // Act
            var result = await service.CreateAsync(dog);

            // Assert
            Assert.True(result);
            var allDogs = await service.ReadAllAsync();
            Assert.Single(allDogs);
        }

        [Fact]
        public async Task ReadAsync_ShouldReturnElement()
        {
            // Arrange
            var service = new CrudServiceAsync<Dog>(_testFilePath);
            var dog = new Dog("Тест", 5, "Лабрадор", true);
            await service.CreateAsync(dog);

            // Act
            var result = await service.ReadAsync(dog.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dog.Id, result.Id);
            Assert.Equal(dog.Name, result.Name);
        }

        [Fact]
        public async Task ReadAllAsync_ShouldReturnAllElements()
        {
            // Arrange
            var service = new CrudServiceAsync<Dog>(_testFilePath);
            await service.CreateAsync(new Dog("Собака1", 3, "Хаскі", false));
            await service.CreateAsync(new Dog("Собака2", 5, "Овчарка", true));
            await service.CreateAsync(new Dog("Собака3", 7, "Бігль", false));

            // Act
            var result = await service.ReadAllAsync();

            // Assert
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task ReadAllAsync_WithPagination_ShouldReturnCorrectPage()
        {
            // Arrange
            var service = new CrudServiceAsync<Dog>(_testFilePath);
            for (int i = 0; i < 10; i++)
            {
                await service.CreateAsync(new Dog($"Собака{i}", i, "Порода", false));
            }

            // Act
            var page0 = await service.ReadAllAsync(0, 3);
            var page1 = await service.ReadAllAsync(1, 3);

            // Assert
            Assert.Equal(3, page0.Count());
            Assert.Equal(3, page1.Count());
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateElement()
        {
            // Arrange
            var service = new CrudServiceAsync<Dog>(_testFilePath);
            var dog = new Dog("Старе ім'я", 3, "Хаскі", false);
            await service.CreateAsync(dog);

            // Act
            dog.Name = "Нове ім'я";
            dog.Age = 5;
            var result = await service.UpdateAsync(dog);

            // Assert
            Assert.True(result);
            var updated = await service.ReadAsync(dog.Id);
            Assert.Equal("Нове ім'я", updated.Name);
            Assert.Equal(5, updated.Age);
        }

        [Fact]
        public async Task RemoveAsync_ShouldRemoveElement()
        {
            // Arrange
            var service = new CrudServiceAsync<Dog>(_testFilePath);
            var dog = new Dog("Тест", 5, "Лабрадор", true);
            await service.CreateAsync(dog);

            // Act
            var result = await service.RemoveAsync(dog);

            // Assert
            Assert.True(result);
            var all = await service.ReadAllAsync();
            Assert.Empty(all);
        }

        [Fact]
        public async Task SaveAsync_ShouldSaveToFile()
        {
            // Arrange
            var service = new CrudServiceAsync<Dog>(_testFilePath);
            await service.CreateAsync(new Dog("Собака1", 3, "Хаскі", false));
            await service.CreateAsync(new Dog("Собака2", 5, "Овчарка", true));

            // Act
            var result = await service.SaveAsync();

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(_testFilePath));
        }

        [Fact]
        public async Task LoadAsync_ShouldLoadFromFile()
        {
            // Arrange
            var service1 = new CrudServiceAsync<Dog>(_testFilePath);
            await service1.CreateAsync(new Dog("Собака1", 3, "Хаскі", false));
            await service1.CreateAsync(new Dog("Собака2", 5, "Овчарка", true));
            await service1.SaveAsync();

            // Act
            var service2 = new CrudServiceAsync<Dog>(_testFilePath);
            var result = await service2.LoadAsync();

            // Assert
            Assert.True(result);
            var loaded = await service2.ReadAllAsync();
            Assert.Equal(2, loaded.Count());
        }

        [Fact]
        public async Task ConcurrentOperations_ShouldBeThreadSafe()
        {
            // Arrange
            var service = new CrudServiceAsync<Dog>(_testFilePath);
            var tasks = new Task[100];

            // Act
            for (int i = 0; i < 100; i++)
            {
                tasks[i] = service.CreateAsync(Dog.CreateNew());
            }
            await Task.WhenAll(tasks);

            // Assert
            var all = await service.ReadAllAsync();
            Assert.Equal(100, all.Count());
        }

        [Fact]
        public void GetEnumerator_ShouldEnumerateElements()
        {
            // Arrange
            var service = new CrudServiceAsync<Dog>(_testFilePath);
            service.CreateAsync(new Dog("Собака1", 3, "Хаскі", false)).Wait();
            service.CreateAsync(new Dog("Собака2", 5, "Овчарка", true)).Wait();

            // Act
            var count = 0;
            foreach (var dog in service)
            {
                count++;
                Assert.NotNull(dog);
            }

            // Assert
            Assert.Equal(2, count);
        }
    }
}


