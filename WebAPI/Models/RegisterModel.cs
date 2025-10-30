using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class RegisterModel
    {
       
        public string UserName { get; set; }
   
        public string HoTen { get; set; }
     
        public string Password { get; set; }
        public string? Email { get; set; }
        public string? MaNhanVien {get; set; }
        public bool? IsAdmin {get; set;}
        public int? Status {get; set;}
        public bool? IsActive {get; set;}
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? CardID { get; set; }
        public string? AvatarImg { get; set; }

    }
}
