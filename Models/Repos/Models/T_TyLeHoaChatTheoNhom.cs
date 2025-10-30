using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("T_TyLeHoaChatTheoNhom")] // This is the table name in the database
public partial class T_TyLeHoaChatTheoNhom
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaNhomHoaChat { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaHoaChat { get; set; } = null!;

    [Column(TypeName = "decimal(18, 3)")]
    public decimal TyLe { get; set; }

    public bool SuDung { get; set; }

    [StringLength(500)]
    public string Ten { get; set; } = null!;
}
