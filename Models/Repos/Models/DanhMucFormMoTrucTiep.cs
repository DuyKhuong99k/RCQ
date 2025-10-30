using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(DanhMucFormMoTrucTiep))] // "DanhMucFormMoTrucTiep
public partial class DanhMucFormMoTrucTiep
{
    [Key]
    [StringLength(100)]
    public string TenForm { get; set; } = null!;

    [StringLength(100)]
    public string? TenHienThi { get; set; }
}
