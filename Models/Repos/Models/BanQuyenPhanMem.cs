using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(BanQuyenPhanMem))]
public partial class BanQuyenPhanMem
{
    [Key]
    [StringLength(100)]
    public string MaMayTinh { get; set; } = null!;

    [StringLength(100)]
    public string? TenMayTinh { get; set; }
}
