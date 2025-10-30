using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(CoiLogs))] // "CoiLogs
public partial class CoiLogs
{
    [Key]
    public long Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaCoi { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string ActionName { get; set; } = null!;

    public DateTime NgayGio { get; set; }

    public int HzQuay { get; set; }

    public int HzRa { get; set; }

    public int TimeQuay { get; set; }

    public bool IsError { get; set; }

    public string? ErrorStr { get; set; }

    public string? Decription { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? IdMonitor { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? NhanVienId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaChatLuong { get; set; }
}
