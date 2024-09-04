using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Models
{
    public class HoaDon
    {
        [Key]
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public applicationUser ApplicationUser { get; set; }
        public double Total { get; set; }
        public DateTime OrderDate { get; set; }
        public String? OrderStatus { get; set; }
        public String PhoneNumber { get; set; }
        public String Name { get; set; }
        public String Address { get; set; }
    }
}
