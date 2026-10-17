using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;

[Table("LichSuXuongMayCan")]
[Index(nameof(MayCan), nameof(TuThoiGian), IsUnique = true)]
public sealed class LichSuXuongMayCan
{
    [Key] public long Id { get; set; }
    [MaxLength(100), Unicode(false)] public string MayCan { get; set; } = "";
    [MaxLength(100)] public string? MaXuong { get; set; }
    public DateTime TuThoiGian { get; set; }
}
