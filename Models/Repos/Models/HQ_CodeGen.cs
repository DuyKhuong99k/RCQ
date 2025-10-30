using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Repos.Models;
[Table(nameof(HQ_CodeGen))]
public class HQ_CodeGen
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    [Column(TypeName = "datetime2(7)")]
    public DateTime NgayTao { get; set; } = DateTime.Now;
    [StringLength(500)]
    public string DangKyIds { get; set; } = null!;

    public bool Printed { get; set; } = false;
}