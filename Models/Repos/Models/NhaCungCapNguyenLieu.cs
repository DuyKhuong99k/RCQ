using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("NhaCungCapNguyenLieu")] // "NhaCungCapNguyenLieu
public partial class NhaCungCapNguyenLieu
{
    [Key]
    [StringLength(50)]
    public string Ma { get; set; } = null!;

    [StringLength(500)]
    public string? Ten { get; set; }

    public bool? SuDung { get; set; }
}
