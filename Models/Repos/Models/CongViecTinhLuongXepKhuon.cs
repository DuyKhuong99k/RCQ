using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(CongViecTinhLuongXepKhuon))] // "CongViecTinhLuongXepKhuon
public partial class CongViecTinhLuongXepKhuon
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(200)]
    public string Ten { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string BravoId { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string? BravoIdDem { get; set; }

    public bool SuDung { get; set; }

    /// <summary>
    /// 1: Lay Tu Bang,2:Nhap Tay,3:Ca 2,4:Lay tu san luong ra coi theo phan bo
    /// </summary>
    public int LoaiDuLieu { get; set; }
}
