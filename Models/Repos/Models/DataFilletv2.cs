using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(DataFilletv2))] // "DataFilletv2
[Keyless]
public partial class DataFilletv2
{
    public double? STT { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Gio { get; set; }

    [StringLength(255)]
    public string? MaNhanVien { get; set; }

    [StringLength(255)]
    public string? MaHoSo { get; set; }

    [StringLength(255)]
    public string? BanNameSL { get; set; }

    [StringLength(255)]
    public string? ThanhPhamName { get; set; }

    [StringLength(255)]
    public string? ThanhPhamNameSL { get; set; }

    [StringLength(255)]
    public string? MaMayCan { get; set; }

    [StringLength(255)]
    public string? MaBan { get; set; }

    [StringLength(255)]
    public string? MaBanSL { get; set; }

    [StringLength(255)]
    public string? MaThanhPham { get; set; }

    [StringLength(255)]
    public string? MaThanhPhamSL { get; set; }

    [StringLength(255)]
    public string? MaNhanVienPhucVu { get; set; }

    [StringLength(255)]
    public string? ThanhPhamFN { get; set; }

    public double? TimesFN { get; set; }

    public double? SecFN { get; set; }

    public double? MinuFN { get; set; }

    [StringLength(255)]
    public string? MaThanhPhamFN { get; set; }
}
