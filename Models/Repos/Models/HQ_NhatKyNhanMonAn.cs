using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(HQ_NhatKyNhanMonAn))]
public class HQ_NhatKyNhanMonAn
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    [Column(TypeName = "datetime2(7)")]
    public DateTime NgayGio { get; set; } = DateTime.Now;
    [StringLength(50)]
    [Unicode(false)]
    public string NhanVienId { get; set; } = null!;
    [StringLength(50)]
    [Unicode(false)]
    public string? ThietBi { get; set; }
    public long CodeId { get; set; }
    [StringLength(500)]
    public string? GhiChu { get; set; }
}