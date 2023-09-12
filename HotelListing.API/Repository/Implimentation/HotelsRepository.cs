using HotelListing.API.Data;
using HotelListing.API.Entity;
using HotelListing.API.Repository.IRepostitory;

namespace HotelListing.API.Repository.Implimentation
{
    public class HotelsRepository: GenericRepository<Hotel>, IHotelsRepository
    {

        public HotelsRepository(HotelListingDbContext context) : base(context)
        {
        }
    }
}
