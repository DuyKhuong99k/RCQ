using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("NhanVienSanLuongTheoLineFillet")] // "NhanVienSanLuongTheoLineFillet
[PrimaryKey("MaNhanVien", "MaLine", "MaXuong", "Ngay")]
public partial class NhanVienSanLuongTheoLineFillet
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaNhanVien { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaLine { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    [Key]
    public DateOnly Ngay { get; set; }
}
