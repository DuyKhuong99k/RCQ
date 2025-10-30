using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;

[Table(nameof(CoiLeXepKhuon))]
public partial class CoiLeXepKhuon
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string MaCoi { get; set; } = null!;
    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string MaSanPham { get; set; } = null!;
    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal TrongLuong { get; set; } = 0M;
    [Required]
    [Column(TypeName = "datetime2(7)")]
    public DateTime NgayGio { get; set; } = DateTime.Now;
    [Required]
    [Column(TypeName = "date")]
    public DateTime NgayNguyenLieu { get; set; } = DateTime.Now.Date;
}