using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(MaLoaiCaNguyenLieu))] // "MaLoaiCaNguyenLieu
public partial class MaLoaiCaNguyenLieu
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(50)]
    public string? Ten { get; set; }

    public bool? SuDung { get; set; }
}
