using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("TheRo")] // This is the table name in the database
public partial class TheRo
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string MaThe { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string ColorCode { get; set; } = null!;

    public DateTime CreatedDateTime { get; set; }

    public bool IsRach { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? MaThanhPhamDinhHinh { get; set; }
}
