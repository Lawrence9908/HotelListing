using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.API.Data.Configuration
{
    public class RoleConfiguration :IEntityTypeConfiguration<IdentityRole>
    {
 
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            
            builder.HasData(
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "User"
                },
                 new IdentityRole
                 {
                     Name = "Administrator",
                     NormalizedName = "ADMINISTRATOR"
                 }
            );
        }
    }
}
