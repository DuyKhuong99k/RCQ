using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vars.Hubs
{
    public class DataLiteRes
    {
        public string Id { get; set; }
        public int STT { get; set; } = 0;
        public string NgayGio { get; set; } = DateTime.Now.ToString("yyyyMMddHHmmss");
        public string? MayCan { get; set; } = "";
        public string? MaLo { get; set; } = "";
        public string? MaSize { get; set; } = "";
        public string? MaThanhPham { get; set; } = "";
        public decimal TrongLuongNhan { get; set; } = 0;
        public decimal TrongLuongTra { get; set; } = 0;
        public decimal TrongLuongYeuCau { get; set; } = 0;
        public decimal DinhMucThucTe { get; set; } = 0;
        public decimal TrongLuongTare { get; set; } = 0;
        public int ChiSanLuong { get; set; } =0;
        public string? MaNhanVien { get; set; }= "";
        public string? MaNhanVienPhucVu { get; set; } = "";
        public string? MaNhanVienBanKiem { get; set; } = "";
        public string? TheId { get; set; } = "";
        public string? TheIdChucNang { get; set; } = "";
        public string? Ngay { get; set; } = DateTime.Now.ToString("yyyyMMdd");
        public int? Status { get; set; } = 0;
        public string? Id2 { get; set; } = "";
        public string? MessStr { get; set; }= "";
        public string? MaLoaiNguyenLieu { get; set; } = "";
        /// <summary>
        /// 0: Không phải thao tác dữ liệu
        /// 1: Là thao tác dữ liệu
        /// </summary>
        public int IsDataAction { get; set; } = 0;
        public decimal TongTrongLuongCan { get; set; } = 0;
        public int TongSoRoCan { get; set; } = 0;
        public decimal TongTrongLuong { get; set; } = 0;
        public int TongSoRo { get; set; } = 0;
    }
}
