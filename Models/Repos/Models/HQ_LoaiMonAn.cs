using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(HQ_LoaiMonAn))]
public partial class HQ_LoaiMonAn
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [StringLength(500)]
    public string Ten { get; set; } = null!;
    [StringLength(50)]
    public string? GhiChu { get; set; }
}