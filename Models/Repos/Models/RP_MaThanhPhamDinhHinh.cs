using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("RP_MaThanhPhamDinhHinh")] // Added
public partial class RP_MaThanhPhamDinhHinh
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    public bool LangDa { get; set; }

    [Column(TypeName = "decimal(18, 3)")]
    public decimal DinhMucLangDa { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string MaNhom { get; set; } = null!;
}
