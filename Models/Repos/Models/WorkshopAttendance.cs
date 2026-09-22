using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Repos.Models;

[Table("ChamCongTaiXuong")]
public sealed class WorkshopAttendance
{
    [Key]
    public long Id { get; set; }

    public DateTime ThoiGian { get; set; }

    [MaxLength(100)]
    public string? Xuong { get; set; }

    [MaxLength(50)]
    public string ThietBi { get; set; } = "";

    [MaxLength(50)]
    public string MaTheTu { get; set; } = "";

    [MaxLength(50)]
    public string? MaNhanVien { get; set; }

    [MaxLength(200)]
    public string? TenNhanVien { get; set; }

    [MaxLength(20)]
    public string? TrangThai { get; set; }
}