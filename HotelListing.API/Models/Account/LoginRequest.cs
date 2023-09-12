using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Models.Account
{
    public class LoginRequest
    {
        public string Email { get; set; }
        [Required]
        [StringLength(15, ErrorMessage = "Your password is limited to a minimu length of 6 and max of 15", MinimumLength = 6)]
        public string Password { get; set; }
    }
}
