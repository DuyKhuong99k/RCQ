using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Repos.HQ
{
    public class CongViecTinhLuongXepKhuonViewModel
    {
        private static CongViecTinhLuongXepKhuonViewModel instance;

        //[ObservableProperty] private ObservableRangeCollection<string> employeeCodes = new();
        public static CongViecTinhLuongXepKhuonViewModel Instance => instance ??= new CongViecTinhLuongXepKhuonViewModel();
        public List<Models.Repos.Models.CongViecTinhLuongXepKhuon> Gets(string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.CongViecTinhLuongXepKhuon(connStr);
                return dao.Gets();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Models.Repos.Models.CongViecTinhLuongXepKhuonSanLuong> Gets(
            DateTime dateTime,
            string xuongId,
            string congViecId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.CongViecTinhLuongXepKhuonSanLuong();
                return dao.Gets(dateTime, xuongId, congViecId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<string> GetThanhPhamIdsPhuXepKhuon(string congViecId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.CongViecTinhLuongXepKhuonTheoLoaiThanhPham(connStr);
                return dao.GetThanhPhamIdsPhuXepKhuon(congViecId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Tuple<string, string>> GetThanhPhamChieuXaIdChinhXepKhuon(string congViecId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.CongViecTinhLuongXepKhuonTheoLoaiThanhPham(connStr);
                return dao.GetThanhPhamChieuXaIdsChinhXepKhuon(congViecId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<string> GetThanhPhamIdsBlockCXepKhuon(string congViecId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.CongViecTinhLuongXepKhuonTheoLoaiThanhPham(connStr);

                return dao.GetThanhPhamIdsBlockXepKhuon(congViecId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<string> GetThanhPhamIdsKHCXepKhuon(string congViecId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.CongViecTinhLuongXepKhuonTheoLoaiThanhPham(connStr);

                return dao.GetThanhPhamIdsKHCXepKhuon(congViecId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<string> GetThanhPhamIdsTaiChe(string congViecId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.CongViecTinhLuongXepKhuonTheoLoaiThanhPham(connStr);
                return dao.GetThanhPhamIdsTaiChe(congViecId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<string> GetCongViecIdsTaiChe(string congViecId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.CongViecTinhLuongXepKhuonTheoLoaiThanhPham(connStr);
                return dao.GetConViecTaiCheIds(congViecId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Tuple<string, string>> GetThanhPhamCongDoanIdsBlockXepKhuon(string congViecId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.CongViecTinhLuongXepKhuonTheoLoaiThanhPham(connStr);

                return dao.GetThanhPhamCongDoanIdsBlockXepKhuon(congViecId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<string> GetThanhPhamIdsTPDinhHinh(string congViecId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.CongViecTinhLuongXepKhuonTheoLoaiThanhPham(connStr);
                return dao.GetThanhPhamIdsTPDinhHinh(congViecId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<string> GetThanhPhamIdsSoChe(string congViecId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.CongViecTinhLuongXepKhuonTheoLoaiThanhPham(connStr);
                return dao.GetThanhPhamIdsSoChe(congViecId);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
