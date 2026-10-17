using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;

/// <summary>Lượt quẹt thuộc ca chung; không tạo thêm giờ công trong CheckInOut.</summary>
[Table(nameof(ScaleAttendanceScan))]
[Index(nameof(VaoId), nameof(MaSoMay), nameof(ThoiGian))]
public class ScaleAttendanceScan
{
    [Key, StringLength(100), Unicode(false)]
    public string Id { get; set; } = "";

    [Required, StringLength(100), Unicode(false)]
    public string VaoId { get; set; } = "";

    public int MaSoMay { get; set; }
    public DateTime ThoiGian { get; set; }

    [Required, StringLength(20)]
    public string TrangThai { get; set; } = "";

    [StringLength(50), Unicode(false)]
    public string? MaThanhPham { get; set; }
}
