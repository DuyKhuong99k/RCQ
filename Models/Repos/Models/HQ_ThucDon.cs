using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(HQ_ThucDon))]
public partial class HQ_ThucDon
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [StringLength(200)] public string Ten { get; set; } = null!;
    [Column(TypeName = "date")]
    public DateTime Ngay { get; set; }
    
    [Column(TypeName = "datetime2(7)")]
    public DateTime NgayTao { get; set; } = DateTime.Now;
    [StringLength(50)]
    [Unicode(false)]
    public string? NguoiTao { get; set; }
    // [StringLength(50)]
    // [Unicode(false)]
    // public int MonAnId { get; set; }
    [StringLength(50)]
    [Unicode(false)]
    public string? ThietBi { get; set; }
    [StringLength(500)]
    public string? GhiChu { get; set; }
    
}