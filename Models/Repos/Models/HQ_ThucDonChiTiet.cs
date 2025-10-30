using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Repos.Models;
[Table(nameof(HQ_ThucDonChiTiet))]
public partial class HQ_ThucDonChiTiet
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public long ThucDonId { get; set; }
    public int MonAnId { get; set; }
    [StringLength(500)]
    public string? GhiChu { get; set; }
}