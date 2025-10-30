using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("RefreshToken")] // Added
public partial class RefreshToken
{
    [Key]
    public Guid Id { get; set; }

    public int UserId { get; set; }

    public string? Token { get; set; }

    public string? JwtId { get; set; }

    public bool IsUsed { get; set; }

    public bool IsRevoked { get; set; }

    public DateTime? IssuedAt { get; set; }

    public DateTime? ExpiredAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("RefreshToken")]
    public virtual NguoiDung User { get; set; } = null!;
}
