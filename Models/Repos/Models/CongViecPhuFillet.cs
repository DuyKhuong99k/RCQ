using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(CongViecPhuFillet))] // "CongViecPhuFillet
public partial class CongViecPhuFillet
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(200)]
    public string Ten { get; set; } = null!;

    public bool PhanSize { get; set; }

    /// <summary>
    /// 1: Xuong, 2: Ban Cat Tiet, 3: Line
    /// </summary>
    public int NguonSanLuong { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? SanPhamId { get; set; }
}
