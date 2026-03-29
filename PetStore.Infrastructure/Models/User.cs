using Microsoft.AspNetCore.Identity;

namespace PetStore.Infrastructure.Models
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}

