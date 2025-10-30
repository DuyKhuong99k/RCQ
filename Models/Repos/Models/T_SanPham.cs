using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("T_SanPham")] // This is the table name in the database
public partial class T_SanPham
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(500)]
    public string Ten { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaLoaiNguyenLieu { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? MaSize { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaThanhPham { get; set; }

    public bool SuDung { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaNL { get; set; }
}
