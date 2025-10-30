using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(DanhSachMayTinh))] // "DanhSachMayTinh
public partial class DanhSachMayTinh
{
    [Key]
    [StringLength(50)]
    public string IDMayTinh { get; set; } = null!;

    [StringLength(50)]
    public string? DiaChiKetNoi { get; set; }

    [StringLength(50)]
    public string? DataSQL { get; set; }

    [StringLength(50)]
    public string? UserSQL { get; set; }

    [StringLength(50)]
    public string? PassSQL { get; set; }

    [StringLength(50)]
    public string? KhuVuc { get; set; }
}
