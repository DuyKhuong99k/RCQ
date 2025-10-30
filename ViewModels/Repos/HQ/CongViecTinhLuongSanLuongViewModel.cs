using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Repos.HQ
{
    public class CongViecTinhLuongSanLuongViewModel
    {
        private static CongViecTinhLuongSanLuongViewModel instance;

        //[ObservableProperty] private ObservableRangeCollection<string> employeeCodes = new();
        public static CongViecTinhLuongSanLuongViewModel Instance => instance ??= new CongViecTinhLuongSanLuongViewModel();
        public List<T> GetsFullField<T>(DateTime dateTime,string maXuong,string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.CongViecTinhLuongXepKhuonSanLuong(connStr);
            return dao.GetsFullField<T>(dateTime,maXuong);
        }
    }
}
