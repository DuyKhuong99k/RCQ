using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("T_Phieu")] // This is the table name in the database
public partial class T_Phieu
{
    [Key]
    public long Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string So { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? MaNhanVien { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? XiNghiep { get; set; }

    public DateTime NgayGio { get; set; }

    public DateTime TuNgayGio { get; set; }

    public DateTime DenNgayGio { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaCongDoan { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal TrongLuong { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal ThanhTien { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UserName { get; set; } = null!;

    [StringLength(50)]
    public string PCName { get; set; } = null!;
}
