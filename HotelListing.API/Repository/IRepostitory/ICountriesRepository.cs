using HotelListing.API.Entity;

namespace HotelListing.API.Repository.IRepostitory
{
    public interface ICountriesRepository:IGenericRepository<Country>
    {
        Task<Country> GetDetails(int id);
    }
}
