using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;

[Table(nameof(MaSizeFillet_U))] // "MaSizeFillet
public partial class MaSizeFillet_U
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [StringLength(50)] [Unicode(false)] public string MaSize { get; set; } = null!;

    [Required]
    [Column(TypeName = "datetime2(7)")]
    public DateTime Ngay { get; set; } = DateTime.Now;

    public bool? SuDung { get; set; }

    [StringLength(50)] public string? Ten { get; set; }
    [Required]
    [Column(TypeName = "datetime2(7)")]
    public DateTime MNgay { get; set; } = DateTime.Now;
}