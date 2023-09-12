using HotelListing.API.Data.Configuration;
using HotelListing.API.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Data
{
    public class HotelListingDbContext: IdentityDbContext<ApplicationUser>
    {

        public HotelListingDbContext(DbContextOptions<HotelListingDbContext> options) : base(options) { }   

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Country> Countries { get; set; }

        //Usef to seed dataToDatabase
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.Entity<Country>().HasData(
                new Country
                {
                    Id = 1,
                    Name = "South Africa",
                    ShortName = "RSA",
                },
                 new Country
                 {
                     Id = 2,
                     Name = "Zimbabwe",
                     ShortName = "Zim",
                 },
                  new Country
                  {
                      Id = 3,
                      Name = "Nigeria",
                      ShortName = "Nig",
                  }

            );

            modelBuilder.Entity<Hotel>().HasData(
               new Hotel
               {
                   Id = 1,  
                   Name = "Sandals Resort and Spa",
                   Address = "Negril",
                   Rating = 2.4,
                   CountryId  = 1

               },
               new Hotel
               {
                   Id = 2,
                   Name = "Khoroni Hotel",
                   Address = "Negril",
                   Rating = 4.4,
                   CountryId = 3

               },
               new Hotel
               {
                   Id = 3,
                   Name = "Nanodi Dem",
                   Address = "Negril",
                   Rating = 3.6,
                   CountryId = 3

               }

            );

        }
    }
}
