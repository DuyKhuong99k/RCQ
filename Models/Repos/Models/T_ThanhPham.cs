using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("T_ThanhPham")] // This is the table name in the database
public partial class T_ThanhPham
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(200)]
    public string Ten { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaKhuVuc { get; set; } = null!;

    public bool SuDung { get; set; }

    public bool IsPhanCo { get; set; }

    public bool IsDem { get; set; }

    public bool ChoPhepChuyenDoiQuyTrinh { get; set; }

    public bool IsHoaChat { get; set; }

    public bool IsThemLo { get; set; }

    public bool IsBatMau { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? DinhMuc { get; set; }

    public bool IsHoaChatMain { get; set; }
}
