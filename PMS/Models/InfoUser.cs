using System.ComponentModel.DataAnnotations;

namespace PMS.Models
{
    public class InfoUser
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string? UserName { get; set; }
        [MaxLength(250)]
        public string? Password { get; set; }
        public string? HoTen { get; set; }
        public string? Email { get; set; }
        public string? DefaultKey { get; set; }
        public string? MaNhanVien { get; set; }
        public bool? IsAdmin { get; set; }
        public int? Status { get; set; }
        public bool? IsActive { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? CardID { get; set; }
        public string? AvatarImg { get; set; }
    }
}
