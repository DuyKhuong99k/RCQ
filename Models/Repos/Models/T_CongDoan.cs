using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("T_CongDoan")] // Added
public partial class T_CongDoan
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(200)]
    public string Ten { get; set; } = null!;

    public bool SuDung { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaLuong { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaLoaiNguyenLieu { get; set; }

    public int Idx { get; set; }
}
