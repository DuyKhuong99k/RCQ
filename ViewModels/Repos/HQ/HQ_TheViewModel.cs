using AppViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models.Repos.Models;
using Models.Repos;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ViewModels.Repos.HQ
{
    public class HQ_TheViewModel
    {
        private static HQ_TheViewModel instance;
        private HQ_TheViewModel()
        {
            try
            {
               

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }

        public static HQ_TheViewModel Instance => instance ??= new HQ_TheViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        
        private List<T> Gets<T>()
        {
            var dao = new Dao.Repos.HQ.HQ_Size();
            return dao.Gets<T>();
        }

        public List<The> GetTheRos(string Ngay, int PageIndex, int PageSize)
        {
            DateTime date = new DateTime();
            try
            {
                date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            var vmTheRo = TheRoViewModel.Instance;
            var items = vmTheRo.Items.Where(x => x.CreatedDateTime > date).OrderBy(x => x.MaThe)
                .Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new The
                {
                    Id = x.MaThe,
                    ColorId = x.ColorCode,
                    MaThanhPham = "",
                    MaSize = "",
                    MaLoaiNguyenLieu = "",
                    MaNhanVien = "",
                    NgayGio = x.CreatedDateTime.ToString("yyyyMMddHHmmss"),
                }).ToList();
            return items;
        }

        public List<The> GetTheNhanViens(string Ngay, int PageIndex, int PageSize)
        {
            DateTime date = new DateTime();
            try
            {
                date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            var vmTheRo = TheViewModel.Instance;
            var items = vmTheRo.Items.Where(x => x.NgayGio > date).OrderBy(x => x.MaTheTu)
                .Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new The
                {
                    Id = x.MaTheTu,
                    ColorId = "",
                    MaThanhPham = "",
                    MaSize = "",
                    MaLoaiNguyenLieu = "",
                    MaNhanVien = x.MaNhanVien??"",
                    NgayGio = x.NgayGio.ToString("yyyyMMddHHmmss"),
                }).ToList();
            return items;
        }
        public List<The> GetTheSizes(string Ngay, int PageIndex, int PageSize)
        {
            DateTime date = new DateTime();
            try
            {
                date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            var vmTheRo = TheThanhPhamViewModel.Instance;
            var items = vmTheRo.Items.Where(x => x.NgayGio > date).OrderBy(x => x.MaThe)
                .Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new The
                {
                    Id = x.MaThe,
                    ColorId = "",
                    MaThanhPham = "",
                    MaSize = x.MaSizeDinhHinh??"",
                    MaLoaiNguyenLieu = "",
                    MaNhanVien = "",
                    NgayGio = x.NgayGio.ToString("yyyyMMddHHmmss"),
                }).ToList();
            return items;
        }
        public List<The> GetTheThanhPhams(string Ngay, int PageIndex, int PageSize)
        {
            DateTime date = new DateTime();
            try
            {
                date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            var vmTheRo = TheThanhPhamViewModel.Instance;
            var items = vmTheRo.Items.Where(x => x.NgayGio > date).OrderBy(x => x.MaThe)
                .Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new The
                {
                    Id = x.MaThe,
                    ColorId = "",
                    MaThanhPham =x.MaThanhPham??"",
                    MaSize ="",
                    MaLoaiNguyenLieu = "",
                    MaNhanVien = "",
                    NgayGio = x.NgayGio.ToString("yyyyMMddHHmmss"),
                }).ToList();
            return items;
        }
        public List<The> GetTheLoaiNguyenLieus(string Ngay, int PageIndex, int PageSize)
        {
            DateTime date = new DateTime();
            try
            {
                date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            var vmTheRo = TheThanhPhamViewModel.Instance;
            var items = vmTheRo.Items.Where(x => x.NgayGio > date).OrderBy(x => x.MaThe)
                .Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => new The
                {
                    Id = x.MaThe,
                    ColorId = "",
                    MaThanhPham = "",
                    MaSize = "",
                    MaLoaiNguyenLieu = x.MaLoaiNguyenLieu??"",
                    MaNhanVien = "",
                    NgayGio = x.NgayGio.ToString("yyyyMMddHHmmss"),
                }).ToList();
            return items;
        }
        public List<string> GetDs(string Ngay,int PageIndex,int PageSize)
        {
            dbPMScontext db = new dbPMScontext();
            DateTime date = new DateTime();
            try
            {
                date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss",CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var items = new List<string>();
            var _items = db.HqTheRoDs.Where(x => x.Ngay > date).OrderByDescending(x => x.Ngay).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x=>x.MaThe).ToList();
            if (_items.Any())
            {
                items.AddRange(_items);
            }
            _items = db.HqTheThanhPhamDs.Where(x => x.Ngay > date).OrderByDescending(x => x.Ngay).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => x.MaThe).ToList();
            if (_items.Any())
            {
                items.AddRange(_items);
            }
            _items = db.HqTheTuDs.Where(x => x.Ngay > date).OrderByDescending(x => x.Ngay).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => x.MaTheTu).ToList();
            if (_items.Any())
            {
                items.AddRange(_items);
            }
            return items;
        }
        public List<string> GetUs(string Ngay,int PageIndex,int PageSize)
        {
            dbPMScontext db = new dbPMScontext();
            DateTime date = new DateTime();
            try
            {
                date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss",CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var items = new List<string>();
            var _items = db.HqTheRoUs.Where(x => x.Ngay > date).OrderByDescending(x => x.Ngay).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x=>x.MaThe).ToList();
            if (_items.Any())
            {
                items.AddRange(_items);
            }
            _items = db.HqTheThanhPhamUs.Where(x => x.Ngay > date).OrderByDescending(x => x.Ngay).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => x.MaThe).ToList();
            if (_items.Any())
            {
                items.AddRange(_items);
            }
            _items = db.HqTheTuUs.Where(x => x.Ngay > date).OrderByDescending(x => x.Ngay).Skip((PageIndex - 1) * PageSize).Take(PageSize).Select(x => x.MaTheTu).ToList();
            if (_items.Any())
            {
                items.AddRange(_items);
            }
            return items;
        }
        public class The
        {
            public string Id { get; set; }
            public string MaNhanVien { get; set; }
            public string MaThanhPham { get; set; }
            public string MaSize { get; set; }
            public string ColorId { get; set; }
            public string NgayGio { get; set; }
            public string MaLoaiNguyenLieu { get; set; }
        }

        
    }
}
