namespace HotelListing.API.Repository.IRepostitory
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> AddAsync(T entity);
        Task<List<T>> GetAllAsync();
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task<T> GetAsync(int id);
        Task<bool> Exists(int id);
    }
}
