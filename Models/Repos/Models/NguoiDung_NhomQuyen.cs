using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("NguoiDung_NhomQuyen")] // "NguoiDung_NhomQuyen
public partial class NguoiDung_NhomQuyen
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(100)]
    public string? Ten { get; set; }
}
