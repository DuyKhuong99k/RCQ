using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Repos.HQ
{
    public class PhieuCanBTPFillet
    {
        private readonly string connectionString;
        public PhieuCanBTPFillet(string? _connectionString = null)
        {
            connectionString = _connectionString ?? AppViewModels.Base.Ins.ConnectionString;
        }
        public List<T> GetPhieuCanDinhHinh_XLPC<T>(DateTime dateTime, string xuongId)
        {
            var query = @"select
p.STT,
p.Ngay,
p.Gio,
p.MaUserCan,
p.MaMayCan,
p.MaLoaiCa,
lc.Ten as LoaiCaName,
p.MaMau,
m.Ten as MauName,
p.MaSize,
s.Ten as SizeName,
p.MaThanhPham,
tp.Ten as ThanhPhamName,
p.MaLo,
p.MaThe,
p.MaNhanVien,
nv.Name as MaNhanVienName,
p.MaMayLangDa,
mld.Ten as MayLangDaName,
p.TrongLuong,
p.IsEnabled,
p.MaXuong,
x.Ten as XuongName,
p.CaTra,
p.GhiChu,
p.TrongLuongTare,
p.MaNhanVienPhucVu,
pv.Name as NhanVienPhucVuName
from PhieuCanBTPFilletv2 p
left join MaLoaiCaFillet lc on p.MaLoaiCa = lc.Ma
left join MaMauFillet m on p.MaMau = m.Ma
left join MaSizeFillet s on p.MaSize = s.Ma
left join MaThanhPhamFillet tp on p.MaThanhPham = tp.Ma
left join NhanVienDaiThanh nv on p.MaNhanVien = nv.MaNhanVien
left join MayLangDa mld on p.MaMayLangDa = mld.Ma
left join XiNghiep x on p.MaXuong = x.Ma
left join NhanVienDaiThanh pv on p.MaNhanVienPhucVu = pv.MaNhanVien
where p.Ngay = @dateTime and p.MaXuong = @xuongId
order by
p.STT desc";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var items = connection.QueryAsync<T>(
                        query,
                        new { ngay = dateTime.Date, xuongId })
                    .Result
                    .ToList();
                return items;
            }
        }
    }
}
