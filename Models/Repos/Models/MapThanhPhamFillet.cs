using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(MapThanhPhamFillet))] // "MapThanhPhamFillet
[PrimaryKey("MaLoaiCaFillet", "MaTPFillet", "MaSize", "IsTangCa")]
public partial class MapThanhPhamFillet
{
    [Key]
    [StringLength(50)]
    public string MaLoaiCaFillet { get; set; } = null!;

    [Key]
    [StringLength(50)]
    public string MaTPFillet { get; set; } = null!;

    [Key]
    [StringLength(50)]
    public string MaSize { get; set; } = null!;

    [StringLength(50)]
    public string? MaBravoFillet { get; set; }

    [Key]
    public bool IsTangCa { get; set; }
}
