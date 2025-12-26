using MongoDB.Driver;
using PetStore.Infrastructure.Models;

namespace PetStore.Infrastructure
{
    public class PetStoreContext
    {
        private readonly IMongoDatabase _database;
        private const string DatabaseName = "PetStoreDB";

        public PetStoreContext(string connectionString)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(DatabaseName);
        }

        // Collections
        public IMongoCollection<DogModel> Dogs => _database.GetCollection<DogModel>("Dogs");
        public IMongoCollection<CatModel> Cats => _database.GetCollection<CatModel>("Cats");
        public IMongoCollection<CustomerModel> Customers => _database.GetCollection<CustomerModel>("Customers");

        // Generic collection getter
        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }
    }
}

