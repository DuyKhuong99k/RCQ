namespace WebAPI.Models
{
    public class TokenModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? Expires { get; set; }
        public double HourExpires { get; set; }
    }
}
