using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("NguoiDung_FormMoTrucTiep")] // "NguoiDung_FormMoTrucTiep
public partial class NguoiDung_FormMoTrucTiep
{
    [Key]
    [StringLength(50)]
    public string Ma { get; set; } = null!;

    [StringLength(100)]
    public string? Ten { get; set; }
}
