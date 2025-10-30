using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("TheTuDaiThanh")] // This is the table name in the database
public partial class TheTuDaiThanh
{
    [Key]
    [StringLength(50)]
    public string MaTheTu { get; set; } = null!;

    [StringLength(50)]
    public string? MaNhanVien { get; set; }
}
