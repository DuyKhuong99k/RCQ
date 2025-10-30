using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("PD_SanPham")] // "PD_SanPham
public partial class PD_SanPham
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(200)]
    public string Ten { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaLoai { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaDonVi { get; set; } = null!;

    public bool SuDung { get; set; }

    public string? GhiChu { get; set; }
}
