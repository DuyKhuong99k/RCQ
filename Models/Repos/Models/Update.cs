using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("Update")] // This is the table name in the database
public partial class Update
{
    [Key]
    [StringLength(100)]
    [Unicode(false)]
    public string TenFile { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? NgayUpdate { get; set; }

    [StringLength(100)]
    public string? MoTa { get; set; }

    public byte[]? TapTin { get; set; }

    [Unicode(false)]
    public string? CheckMD5 { get; set; }
}
