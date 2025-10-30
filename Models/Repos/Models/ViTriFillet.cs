using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("ViTriFillet")] // This is the table name in the database
public partial class ViTriFillet
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(200)]
    public string Ten { get; set; } = null!;

    public bool SuDung { get; set; }

    public bool MacDinh { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string CodeId { get; set; } = null!;
}
