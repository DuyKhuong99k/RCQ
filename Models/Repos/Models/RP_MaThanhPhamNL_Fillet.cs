using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("RP_MaThanhPhamNL_Fillet")] // Added
public partial class RP_MaThanhPhamNL_Fillet
{
    [StringLength(50)]
    [Unicode(false)]
    public string MaThanhPhamNguyenLieu { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaThanhPhamFillet { get; set; } = null!;

    [Column(TypeName = "decimal(18, 3)")]
    public decimal DinhMuc { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal DinhMucCatTiet { get; set; }
}
