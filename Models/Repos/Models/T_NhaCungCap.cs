using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("T_NhaCungCap")] // Added
public partial class T_NhaCungCap
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(500)]
    public string Ten { get; set; } = null!;

    [StringLength(500)]
    public string? DiaChi { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? SoDienThoai { get; set; }

    public bool SuDung { get; set; }
}
