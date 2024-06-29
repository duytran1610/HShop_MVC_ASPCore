using System.ComponentModel.DataAnnotations;

namespace HShop.ViewModels
{
    public class LoginVM
    {
        [Display(Name ="Account")]
        [Required(ErrorMessage ="Account empty")]
        [MaxLength(20, ErrorMessage = "Max length 20 chars")]
        public string MaKh { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password empty")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }
    }
}
