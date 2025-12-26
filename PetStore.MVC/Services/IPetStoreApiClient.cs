namespace PetStore.MVC.Services
{
    public interface IPetStoreApiClient
    {
        Task<T?> GetAsync<T>(string endpoint);
        Task<IEnumerable<T>> GetListAsync<T>(string endpoint);
        Task<bool> PostAsync<T>(string endpoint, T data);
        Task<bool> PutAsync<T>(string endpoint, T data);
        Task<bool> DeleteAsync(string endpoint);
    }
}

