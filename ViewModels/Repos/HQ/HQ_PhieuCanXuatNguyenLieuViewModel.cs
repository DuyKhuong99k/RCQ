using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Repos.HQ
{
    public class HQ_PhieuCanXuatNguyenLieuViewModel
    {
        private static HQ_PhieuCanXuatNguyenLieuViewModel instance;
        public static HQ_PhieuCanXuatNguyenLieuViewModel Instance => instance ??= new HQ_PhieuCanXuatNguyenLieuViewModel();
        private HQ_PhieuCanXuatNguyenLieuViewModel()
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<T> GetChiTietPhieuCaXuatNguyenLieus<T>(DateTime fromDate, DateTime toDate, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCanXuatNguyenLieu(connStr);
            return dao.GetChiTietPhieuCaXuatNguyenLieus<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetTongHopSanPhamPhieuCanXuatNguyenLieus<T>(DateTime fromDate, DateTime toDate, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCanXuatNguyenLieu(connStr);
            return dao.GetTongHopSanPhamPhieuCanXuatNguyenLieus<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetTonKhoSanPhamPhieuCanXuatNguyenLieus<T>(DateTime fromDate, DateTime toDate, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCanXuatNguyenLieu(connStr);
            return dao.GetTonKhoSanPhamPhieuCanXuatNguyenLieus<T>(fromDate, toDate, xuongId);
        }

        public List<T> GetTyLeNguyenLieuHaoHut<T>(DateTime ngay, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCanXuatNguyenLieu(connStr);
            return dao.GetTyLeNguyenLieuHaoHut<T>(ngay);
        }


        public List<T> GetKhoiLuongXuatLoTheoXuonget<T>(string maLo, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCanXuatNguyenLieu(connStr);
            return dao.GetKhoiLuongXuatLoTheoXuonget<T>(maLo);
        }
        public List<T> GetListLo<T>(string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCanXuatNguyenLieu(connStr);
            return dao.GetListLo<T>();
        }
    }
}
