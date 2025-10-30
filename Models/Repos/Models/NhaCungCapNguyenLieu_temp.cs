using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("NhaCungCapNguyenLieu_temp")] // "NhaCungCapNguyenLieu_temp
public partial class NhaCungCapNguyenLieu_temp
{
    [Key]
    [StringLength(50)]
    public string Ma { get; set; } = null!;

    [StringLength(500)]
    public string? Ten { get; set; }

    public bool? SuDung { get; set; }
}
