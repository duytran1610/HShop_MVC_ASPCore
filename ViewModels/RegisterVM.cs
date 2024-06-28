using System.ComponentModel.DataAnnotations;

namespace HShop.ViewModels
{
    public class RegisterVM
    {
        [Display(Name = "Account")]
        [Required(ErrorMessage = "*")]
        [MaxLength(20, ErrorMessage ="Max length 20 chars")]
        public string MaKh { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "*")]
        [MinLength(8, ErrorMessage = "Max length 8 chars")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }

        [Display(Name = "Full Name")]
        [Required(ErrorMessage = "*")]
        [MaxLength(50, ErrorMessage = "Max length 50 chars")]
        public string HoTen { get; set; }

        [Display(Name = "Sex")]
        public bool GioiTinh { get; set; } = true;

        [Display(Name = "Birthday")]
        [DataType(DataType.Date)]
        public DateTime NgaySinh { get; set; } = DateTime.Now;

        [Display(Name = "Address")]
        [MaxLength(60, ErrorMessage = "Max length 60 chars")]
        public string? DiaChi { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Phone")]
        [MaxLength(24, ErrorMessage = "Max length 24 chars")]
        [RegularExpression(@"0[9875]\d{8}", ErrorMessage ="Format phone invalid")]
        public string? DienThoai { get; set; }

        [Required(ErrorMessage = "*")]
        [EmailAddress(ErrorMessage ="Format email invalid")]
        public string Email { get; set; }
    }
}
