using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("NhanVienDaiThanhCu")] // "NhanVienDaiThanhCu
public partial class NhanVienDaiThanhCu
{
    [Key]
    [StringLength(50)]
    public string MaNhanVien { get; set; } = null!;

    [StringLength(50)]
    public string? MaChamCong { get; set; }

    [StringLength(50)]
    public string? MaHoSo { get; set; }

    [StringLength(50)]
    public string? Xuong { get; set; }

    [StringLength(100)]
    public string? Name { get; set; }

    [StringLength(50)]
    public string? BirthDate { get; set; }

    [StringLength(50)]
    public string? DeptCode0 { get; set; }

    [StringLength(50)]
    public string? DeptName0 { get; set; }

    [StringLength(50)]
    public string? ChucVu { get; set; }

    [StringLength(50)]
    public string? GenderName { get; set; }

    [StringLength(50)]
    public string? Tel { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }

    [StringLength(50)]
    public string? JobPositionName0 { get; set; }

    [StringLength(50)]
    public string? FirstWorkingDate { get; set; }
}
