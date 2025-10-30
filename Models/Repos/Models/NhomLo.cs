using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;
[Table("NhomLo")] // "NhomLo
public partial class NhomLo
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Id { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string Ma { get; set; } = null!;

    [StringLength(50)]
    public string Ten { get; set; } = null!;

    public DateOnly Ngay { get; set; }

    public DateOnly NgayTao { get; set; }

    public TimeOnly GioTao { get; set; }

    public int TrangThai { get; set; }
}
