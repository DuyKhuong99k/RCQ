using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MvvmHelpers;

namespace Models.Repos.Models;
[Table("PLCChiTiet")] // Added
public partial class PLCChiTiet
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string Id { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaCoi { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string MaXuong { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string PLCId { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string RUN { get; set; } = null!;
    [StringLength(50)]
    [Required]
    [Unicode(false)]
    public string RUNOUT { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string STOP { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string TimeQuay { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string HzQuay { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string HzRa { get; set; } = null!;
    [Column(TypeName = "int")]
    public ushort TimeQuayDef { get; set; }
    [Column(TypeName = "int")]
    public ushort HzQuayDef { get; set; }
    [Column(TypeName = "int")]
    public ushort HzRaDef { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string INVERTER { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string RUNSTATUS { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string PAUSE { get; set; } = null!;
    /// <summary>
    /// -1 đang dừng
    /// 0 đang chờ
    /// 1 đang quay
    /// 2 tam dừng
    /// 3 đang ra
    /// 4 chờ ra
    /// </summary>
    [NotMapped]
    public int State { get; set; } = -1;
    [NotMapped]
    public DateTime? TimeRun { get; set; }
    [NotMapped]
    public DateTime? TimeRaCoi { get; set; }
    [NotMapped]
    public DateTime? TimeStop { get; set; }
    [NotMapped]
    public ushort ThoiGianQuay {get; set; }
    [NotMapped]
    public ushort TanSoQuay {get; set; }
    [NotMapped]
    public ushort TanSoRa {get; set; }
    [NotMapped]
    public bool IsConnected { get; set; }

    [NotMapped] public string SubTitle => $"{MaCoi} - {MaXuong}";
    [NotMapped] public bool IsPowerOn { get; set; } = false;
    [NotMapped] public bool IsPause { get; set; } = false;
    [NotMapped] public bool IsRunOut { get; set; } = false;
    [NotMapped] public ObservableRangeCollection<CoiLogs> Logs { get; set; } = new();
    [NotMapped] public ObservableRangeCollection<object> LiteReports { get; set; } = new();
    [NotMapped] public string? IdMonitor { get; set; }
    [NotMapped] public decimal TrongLuong { get; set; }
    [NotMapped] public string? NhanVienId { get; set; }
    [NotMapped] public string? MaChatLuong { get; set; }
    [NotMapped] public string? PCQuay { get; set; }
    [NotMapped] public string? TheId {get; set; }
    /// <summary>
    /// Item1: Ten Coi Tam
    /// Item2: Ten May Can
    /// </summary>
    [NotMapped]  public List<Tuple <string,string>> CoiTamInfos { get; set; }
}
