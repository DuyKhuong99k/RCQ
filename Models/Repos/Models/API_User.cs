using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Repos.Models;

[Keyless]
[Table(nameof(API_User))]
public partial class API_User
{
    [StringLength(50)]
    [Unicode(false)]
    public string UserName { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Unicode(false)]
    public string Password { get; set; } = null!;

    public bool SuDung { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string DefaultPassword { get; set; } = null!;
}
