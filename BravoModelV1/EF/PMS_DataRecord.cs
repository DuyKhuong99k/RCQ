namespace BravoModelV1.EF
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class PMS_DataRecord
    {
        [StringLength(50)] public string ID { get; set; }

        [Required] [StringLength(50)] public string CMND { get; set; }

        public DateTime ThoiGian { get; set; }

        public bool Status { get; set; }

        [StringLength(50)] public string CongDoanID { get; set; }

        public decimal? TrongLuong { get; set; }

        public DateTime DateCreate { get; set; }

        public DateTime? DateSync { get; set; }
    }
}