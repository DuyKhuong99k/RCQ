using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Models.Repos.Models
{
    [Table("HQ_QuyCachNguyenLieu")]
    public class HQ_QuyCachNguyenLieu
    {
        [Key]
        [Column("Id", TypeName = "bigint")]
        public long Id { get; set; }

        [Column("Ten", TypeName = "nvarchar(500)")]
        public string Ten { get; set; }

        [Column("SuDung", TypeName = "bit")]
        public bool SuDung { get; set; }

        [Column("MNgay", TypeName = "datetime2(7)")]
        public DateTime MNgay { get; set; }

        [Column("Index", TypeName = "int")]
        public int? Index { get; set; }

        [Column("NhomQuyCach", TypeName = "nvarchar(50)")]
        public string? NhomQuyCach { get; set; }

        [Column("MaNguyenLieu", TypeName = "nvarchar(50)")]
        public string? MaNguyenLieu { get; set; }

        [Column("DonViTinh", TypeName = "nvarchar(50)")]
        public string? DonViTinh { get; set; }
    }
}
