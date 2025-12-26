using MongoDB.Driver;
using System.Linq.Expressions;

namespace PetStore.Infrastructure.Repository
{
    public class MongoRepository<T> : IRepository<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(IMongoCollection<T> collection)
        {
            _collection = collection;
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id.ToString());
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException("Entity must have an Id property");

            var idValue = idProperty.GetValue(entity);
            if (idValue == null)
                throw new InvalidOperationException("Entity Id cannot be null");

            var filter = Builders<T>.Filter.Eq("_id", idValue.ToString());
            await _collection.ReplaceOneAsync(filter, entity);
        }

        public async Task DeleteAsync(T entity)
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException("Entity must have an Id property");

            var idValue = idProperty.GetValue(entity);
            if (idValue == null)
                throw new InvalidOperationException("Entity Id cannot be null");

            var filter = Builders<T>.Filter.Eq("_id", idValue.ToString());
            await _collection.DeleteOneAsync(filter);
        }
    }
}

