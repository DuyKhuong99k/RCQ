using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("PMS_L")] // Added
public partial class PMS_L
{
    [Key]
    public int STT { get; set; }

    [Unicode(false)]
    public string SCode { get; set; } = null!;

    [Unicode(false)]
    public string Par1 { get; set; } = null!;

    [Unicode(false)]
    public string Par2 { get; set; } = null!;

    [Unicode(false)]
    public string Par3 { get; set; } = null!;

    [Unicode(false)]
    public string Par4 { get; set; } = null!;
}
