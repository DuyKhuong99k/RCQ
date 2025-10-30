using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("T_PhieuPhanCo")] // This is the table name in the database
public partial class T_PhieuPhanCo
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(200)]
    public string Ten { get; set; } = null!;

    public bool SuDung { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime NgayTao { get; set; }

    public string? GhiChu { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaThanhPham { get; set; }

    [StringLength(200)]
    public string? LoaiPhieuPhanCo { get; set; }

    public int STT { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string? MaNhomYeuCau { get; set; }
}
