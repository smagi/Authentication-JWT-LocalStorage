using Microsoft.AspNetCore.Identity;

namespace WebAuth.Api.Entities
{
    public class ApplicationRole: IdentityRole<Guid>
    { 
        public ApplicationRole(string name)
        {
            Name = name;
        }
    }

}