using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace Demo.Models
{
    public class applicationUser: IdentityUser
    {
        [Required]
        public string Name { get; set; }
        public string? Address { get; set; }
    }
}
