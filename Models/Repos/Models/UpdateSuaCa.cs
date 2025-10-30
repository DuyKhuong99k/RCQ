using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("UpdateSuaCa")] // This is the table name in the database
[PrimaryKey("TenFile", "TenFolder", "ComputerName")]
public partial class UpdateSuaCa
{
    [Key]
    [StringLength(200)]
    [Unicode(false)]
    public string TenFile { get; set; } = null!;

    [Key]
    [StringLength(200)]
    [Unicode(false)]
    public string TenFolder { get; set; } = null!;

    [Key]
    [StringLength(200)]
    [Unicode(false)]
    public string ComputerName { get; set; } = null!;

    public DateOnly Ngay { get; set; }

    public TimeOnly Time { get; set; }

    public byte[] FileData { get; set; } = null!;

    [StringLength(200)]
    [Unicode(false)]
    public string MD5Code { get; set; } = null!;
}
