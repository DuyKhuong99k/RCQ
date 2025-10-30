using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;

[Table(nameof(NguoiDung))]
public partial class NguoiDung
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UserName { get; set; } = null!;

    [StringLength(250)]
    [Unicode(false)]
    public string Password { get; set; } = null!;

    [StringLength(150)]
    public string? HoTen { get; set; }

    [StringLength(150)]
    public string? Email { get; set; }

    [StringLength(150)]
    public string DefaultKey { get; set; } = null!;

    [StringLength(250)]
    public string? MaNhanVien { get; set; }

    public bool? IsAdmin { get; set; }

    /// <summary>
    /// // 1: ĐANG LÀM, 2: TẠM NGHĨ, 3: THÔI VIỆC, 4: THỬ VIỆC
    /// </summary>
    public int? Status { get; set; }

    public bool? IsActive { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string? CardID { get; set; }

    public string? AvatarImg { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<RefreshToken> RefreshToken { get; set; } = new List<RefreshToken>();

    [InverseProperty("User")]
    public virtual ICollection<UserRole> UserRole { get; set; } = new List<UserRole>();
}
