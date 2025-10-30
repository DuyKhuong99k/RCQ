using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("Role")] // Added
public partial class Role
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    [InverseProperty("Role")]
    public virtual ICollection<UserRole> UserRole { get; set; } = new List<UserRole>();
}
