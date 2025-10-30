using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("TheTu")] // This is the table name in the database
public partial class TheTu
{
    [Key]
    [StringLength(50)]
    public string MaTheTu { get; set; } = null!;

    [StringLength(50)]
    public string? MaNhanVien { get; set; }

    public DateTime NgayGio { get; set; }

    [StringLength(50)]
    public string PCName { get; set; } = null!;
    [NotMapped]
    public string? NhanVienName { get; set; }
    [NotMapped]
    public string? MaSo { get; set; }
}
