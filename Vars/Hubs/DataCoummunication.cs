using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vars.Hubs
{
    public class DataCoummunication 
    {
        public string? IdIn { get; set; }
        public string? IdOut { get; set; }
        public string? TheId { get; set; }
        public decimal TrongLuong { get; set; }
        public decimal TrongLuongTare { get; set; }
        public decimal DinhMuc { get; set; }
        public string? MayCan { get; set; }
        public string? NgayGio { get; set; }
        public string MaLo { get; set; }
        public string? MaSize { get; set; }
        public string MaThanhPham { get; set; }
        public string? MaNhanVien { get; set;}
        public string? MaNhanVienKiem { get; set; }
        public string? MaNhanVienPhucVu { get; set; }
        public string? TheChucNangId { get; set; }
        /// <summary>
        /// 0: Mới tạo
        /// 1: Đã Đồng Bộ
        /// 2: Đã Cập Nhật
        /// -1:Đã Hủy 
        /// </summary>
        public int Status { get; set; }
       

    }
}
