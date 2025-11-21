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
    [Table("HQ_KhoNguyenLieu")]
    public class HQ_KhoNguyenLieu
    {
        [Key]
        [Column("Id", TypeName = "bigint")]
        public long Id { get; set; }

        [Column("Ten", TypeName = "nvarchar(500)")]
        public string Ten { get; set; }

        [Column("SuDung", TypeName = "bit")]
        public bool SuDung { get; set; }

        [Column("DiaChi", TypeName = "nvarchar(1000)")]
        public string DiaChi { get; set; }
    }
}
