using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class TheLoai
    {
        [Key]
        public int Id { get; set; } // Khóa chính
        [Required(ErrorMessage ="Không được để trống!")] // bắt buộc ( not null "k đc phép null")
        [Display(Name="Thể Loại")]


        public string Name { get; set; }
        [Required(ErrorMessage ="Không được để trống XD!")]
        [Display(Name ="Ngày tạo")]
        public DateTime DateCreated { get; set; } = DateTime.Now;
        // Date time ko dùng đc required vì nó nhập dd/yy/mmm rồi nên k đc

        //public bool IsDeleted { get; set; }// back up du lieu
    }
}
