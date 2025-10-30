using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(MaSizeNguyenLieu_temp))] // "MaSizeNguyenLieu_temp
public partial class MaSizeNguyenLieu_temp
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(50)]
    public string? Ten { get; set; }

    public bool? SuDung { get; set; }
}
