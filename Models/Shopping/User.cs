using Microsoft.AspNetCore.Identity;

namespace Monitor_2.Models.Shopping
{
    public class User : IdentityUser<int>  // Use IdentityUser with int as key type
    {
        public string Nickname { get; set; }
        public DateTime RegistrationDate { get; set; }

        public ICollection<User_Search> UserSearches { get; set; } = default!;
        public ICollection<User_Lot> UserLots { get; set; } = default!;
    }
}
