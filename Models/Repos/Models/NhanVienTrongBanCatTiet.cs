using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("NhanVienTrongBanCatTiet")] // "NhanVienTrongBanCatTiet
[PrimaryKey("MaBanCatTiet", "MaNhanVien")]
public partial class NhanVienTrongBanCatTiet
{
    [Key]
    [StringLength(50)]
    public string MaBanCatTiet { get; set; } = null!;

    [Key]
    [StringLength(50)]
    public string MaNhanVien { get; set; } = null!;
}
