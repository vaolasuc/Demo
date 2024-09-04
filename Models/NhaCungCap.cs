using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class NhaCungCap
    {
        [Key]
        public int Id { get; set; } // Khóa chính
        [Required(ErrorMessage = "Không được để trống!")] // bắt buộc ( not null "k đc phép null")
        [Display(Name = "NhaCungcap")] // bắt buộc ( not null "k đc phép null")

        public string Name { get; set; }
        [Required(ErrorMessage = "Không được để trống!")] // bắt buộc ( not null "k đc phép null")
        [Display(Name = "Name")]

        public string DiaChi { get; set; }
        [Required(ErrorMessage = "Không được để trống!")] // bắt buộc ( not null "k đc phép null")
        [Display(Name = "Dia Chi")]

        public string SDT { get; set; }
        
    } 
}
