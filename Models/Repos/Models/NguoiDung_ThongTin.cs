using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("NguoiDung_ThongTin")] // "NguoiDung_ThongTin
public partial class NguoiDung_ThongTin
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string TenNguoiDung { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? MaNhanVien { get; set; }

    [StringLength(50)]
    public string? MatKhau { get; set; }

    public bool SuDung { get; set; }

    [StringLength(50)]
    public string? MoFormTrucTiep { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string FolderName { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string FileName { get; set; } = null!;
}
