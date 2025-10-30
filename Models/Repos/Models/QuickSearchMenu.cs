using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("QuickSearchMenu")] // Added
public partial class QuickSearchMenu
{
    [Key]
    public int Id { get; set; }

    [StringLength(500)]
    public string Name { get; set; } = null!;

    [Unicode(false)]
    public string Path { get; set; } = null!;

    public int UserId { get; set; }

    public bool SuDung { get; set; }
}
