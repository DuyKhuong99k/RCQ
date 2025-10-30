using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(HQ_DuyetThucDon))]
public partial class HQ_DuyetThucDon
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    [Column(TypeName = "datetime2(7)")]
    public DateTime NgayTao { get; set; } = DateTime.Now;
    [StringLength(50)]
    [Unicode(false)]
    public string? NguoiTao { get; set; }
    public long ThucDonId { get; set; }
    [StringLength(500)]
    public string? GhiChu { get; set; }
}