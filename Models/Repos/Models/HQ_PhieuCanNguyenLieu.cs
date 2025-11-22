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
    [Table("HQ_PhieuCanNguyenLieu")]
    public class HQ_PhieuCanNguyenLieu
    {
        [Key]
        [Column("Id", TypeName = "varchar(50)")]
        public string Id { get; set; }                         

        [Column("STT", TypeName = "int")]
        public int STT { get; set; }                         

        [Column("Ngay", TypeName = "date")]
        public DateTime Ngay { get; set; }                   

        [Column("NgayGio", TypeName = "datetime2")]
        public DateTime NgayGio { get; set; }                

        [Column("MayCan" , TypeName = "varchar(50)")]
        [MaxLength(50)]
        public string? MayCan { get; set; }                  

        [Column("MaLo", TypeName = "varchar(50)")]
        [MaxLength(50)]
        public string? MaLo { get; set; }                    

        [Column("GhiChu", TypeName = "nvarchar(max)")]
        public string? GhiChu { get; set; }                  

        [Column("MaXuong", TypeName = "varchar(50)")]
        [MaxLength(50)]
        public string? MaXuong { get; set; }                 
    }
}
