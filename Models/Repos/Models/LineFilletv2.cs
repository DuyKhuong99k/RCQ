using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table(nameof(LineFilletv2))] // "LineFilletv2
public partial class LineFilletv2
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(500)]
    public string Ten { get; set; } = null!;

    public bool SuDung { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string CodeId { get; set; } = null!;
}
