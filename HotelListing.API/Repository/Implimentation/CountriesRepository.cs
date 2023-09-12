using HotelListing.API.Data;
using HotelListing.API.Entity;
using HotelListing.API.Repository.IRepostitory;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Repository.Implimentation
{
    public class CountriesRepository : GenericRepository<Country>, ICountriesRepository
    {
        private readonly HotelListingDbContext _context;

        public CountriesRepository(HotelListingDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Country> GetDetails(int id)
        {
            return await _context.Countries.Include(q => q.Hotels).FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
