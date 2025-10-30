using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Models.Repos;
using Models.Repos.Models;

namespace ViewModels.Repos.HQ
{
    public partial class HQ_NhanVienViewModel : ObservableObject
    {
        private static HQ_NhanVienViewModel instance;
        public static HQ_NhanVienViewModel Instance => instance ??= new HQ_NhanVienViewModel();
        private HQ_NhanVienViewModel()
        {
            try
            {
                //Reload();

            }
            catch (Exception e)
            {
                
            }
        }
        public List<HQ_NhanVien_D> GetDs(string Ngay,int PageIndex,int PageSize)
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
            return db.HqNhanVienDs.Where(x=>x.MNgay > date).OrderByDescending(x=>x.MNgay).Skip((PageIndex -1)*PageSize).Take(PageSize).ToList();
        }

        public List<HQ_NhanVien_U> GetUs(string Ngay,int PageIndex,int PageSize)
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
            return db.HqNhanVienUs.Where(x=>x.MNgay > date).OrderByDescending(x=>x.MNgay).Skip((PageIndex -1)*PageSize).Take(PageSize).ToList();
        }
    }
}
