using System.ComponentModel.DataAnnotations;

namespace PMS.Models
{
    public class UserInfo
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public double Hour { get; set; }
    }
}
