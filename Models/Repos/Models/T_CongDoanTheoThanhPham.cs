using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("T_CongDoanTheoThanhPham")] // Added
public partial class T_CongDoanTheoThanhPham
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaThanhPham { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaCongDoan { get; set; } = null!;

    public bool SuDung { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal Idx { get; set; }
}
