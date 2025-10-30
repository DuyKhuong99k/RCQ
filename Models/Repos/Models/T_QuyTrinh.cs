using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("T_QuyTrinh")] // This is the table name in the database
public partial class T_QuyTrinh
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(200)]
    public string Ten { get; set; } = null!;

    public bool SuDung { get; set; }

    public string? GhiChu { get; set; }

    public bool IsNguyenLieu { get; set; }

    [StringLength(200)]
    public string? LoaiQuyTrinh { get; set; }

    [StringLength(200)]
    public string? MaSizeVoXo { get; set; }
}
