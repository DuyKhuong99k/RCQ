using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Repos.Models;

[Table("ThietBiChamCongTaiXuong")]
public sealed class WorkshopDevice
{
    [Key] public string DeviceCode { get; set; } = "";
    [MaxLength(200)] public string DeviceName { get; set; } = "";
    [MaxLength(100)] public string WorkshopCode { get; set; } = "";
    [MaxLength(200)] public string? ConnectionId { get; set; }
    [MaxLength(50)] public string? IpAddress { get; set; }
    public bool IsOnline { get; set; }
    public DateTime? LastSeen { get; set; }
}
