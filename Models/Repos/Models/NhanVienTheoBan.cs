using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("NhanVienTheoBan")] // "NhanVienTheoBan
public partial class NhanVienTheoBan
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Id { get; set; } = null!;

    public DateTime NgayGio { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaNhanVien { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaBan { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaKhuVuc { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string PCName { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? UserName { get; set; }

    public bool IsDone { get; set; }
}
