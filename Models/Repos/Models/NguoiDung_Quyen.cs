using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("NguoiDung_Quyen")] // "NguoiDung_Quyen
[PrimaryKey("TenNguoiDung", "NhomNguoiDung", "MaXuong")]
public partial class NguoiDung_Quyen
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string TenNguoiDung { get; set; } = null!;

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string NhomNguoiDung { get; set; } = null!;

    public bool Xem { get; set; }

    public bool Them { get; set; }

    public bool Sua { get; set; }

    public bool Xoa { get; set; }

    public bool KetChuyen { get; set; }

    public bool CaiDat { get; set; }

    public bool ChamGio { get; set; }

    public int Loai { get; set; }

    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;
}
