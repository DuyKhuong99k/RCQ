using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("RP_MaThanhPhamNL_SoChe")] // Added
public partial class RP_MaThanhPhamNL_SoChe
{
    [StringLength(50)]
    [Unicode(false)]
    public string MaThanhPhamNL { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaThanhPhamSoChe { get; set; } = null!;

    [Column(TypeName = "decimal(18, 3)")]
    public decimal DinhMucCatTiet { get; set; }
}
