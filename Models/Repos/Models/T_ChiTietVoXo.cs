using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("T_ChiTietVoXo")] // Added
public partial class T_ChiTietVoXo
{
    [Key]
    [StringLength(200)]
    [Unicode(false)]
    public string Id { get; set; } = null!;

    [StringLength(200)]
    public string? MaPhieuPhanCo { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal VoXo { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaLoaiNguyenLieu { get; set; }

    /// <summary>
    /// Nhóm Conong Việc
    /// </summary>
    [StringLength(50)]
    [Unicode(false)]
    public string? MaThanhPham { get; set; }

    /// <summary>
    /// Công việc
    /// </summary>
    [StringLength(50)]
    [Unicode(false)]
    public string? MaCongDoan { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaSize { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaQuyTrinh { get; set; }

    public DateTime? NgayGio { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? UserName { get; set; }

    [StringLength(200)]
    public string? PCName { get; set; }

    [StringLength(200)]
    public string? MaSizeVoXo { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal TrongLuong { get; set; }

    [StringLength(50)]
    public string? VoXo2 { get; set; }
}
