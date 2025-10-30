using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Repos.HQ
{
    public class CongViecTinhLuongTheoLoaiThanhPhamViewModel
    {
        private static CongViecTinhLuongTheoLoaiThanhPhamViewModel instance;

        //[ObservableProperty] private ObservableRangeCollection<string> employeeCodes = new();
        public static CongViecTinhLuongTheoLoaiThanhPhamViewModel Instance => instance ??= new CongViecTinhLuongTheoLoaiThanhPhamViewModel();
        public List<T> GetsFullField<T>(string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.CongViecTinhLuongXepKhuonTheoLoaiThanhPham(connStr);
            return dao.GetsFullField<T>();
        }
    }
}
