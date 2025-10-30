using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("NhaCungCapNguyenLieu_DonGiaVanChuyen")] // "NhaCungCapNguyenLieu_DonGiaVanChuyen
[PrimaryKey("MaNhaCC", "NgayApDung")]
public partial class NhaCungCapNguyenLieu_DonGiaVanChuyen
{
    [Key]
    [StringLength(50)]
    public string MaNhaCC { get; set; } = null!;

    [Key]
    [Column(TypeName = "date")]
    public DateTime NgayApDung { get; set; }

    [Column(TypeName = "decimal(18, 1)")]
    public decimal? DonGia { get; set; }

    [StringLength(500)]
    public string? GhiChu { get; set; }
}
