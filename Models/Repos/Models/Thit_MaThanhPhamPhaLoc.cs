using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("Thit_MaThanhPhamPhaLoc")] // This is the table name in the database
public partial class Thit_MaThanhPhamPhaLoc
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(200)]
    public string Ten { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaLoaiNguyenLieu { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? MaLuong { get; set; }

    public string? GhiChu { get; set; }

    public bool SuDung { get; set; }

    public bool IsMang { get; set; }

    public bool IsThit { get; set; }

    public bool IsXuong { get; set; }

    public bool IsMo { get; set; }

    public bool IsChanGio { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? CodeId { get; set; }
}
