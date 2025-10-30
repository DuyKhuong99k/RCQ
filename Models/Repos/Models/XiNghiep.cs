using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("XiNghiep")] // This is the table name in the database
public partial class XiNghiep
{
    [Key]
    [StringLength(50)]
    public string Ma { get; set; } = null!;

    [StringLength(50)]
    public string? Ten { get; set; }

    public bool? SuDung { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string CodeId { get; set; } = null!;
}
