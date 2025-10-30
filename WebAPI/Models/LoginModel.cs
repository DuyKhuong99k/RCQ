using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class LoginModel
    {
        [Required]
        [MaxLength(50)]
        public string UserName {get; set;}
        [MaxLength(250)]
        public string Password {get; set;}
        public bool RememberMe {get; set;}
        public double Hour {get; set;}
    }
}
