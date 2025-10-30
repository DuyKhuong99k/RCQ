using AppModels;
using AppViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models.Repos.Models;
using MvvmHelpers;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Azure.Identity;
using System.Collections.Specialized;
using Vars.Hubs;
using Microsoft.Data.SqlClient;
using System.Data;
namespace ViewModels.Repos.HQ
{
    public partial class PhieuCanKeToanViewModel
    {
        private static PhieuCanKeToanViewModel instance;

        //[ObservableProperty] private ObservableRangeCollection<string> employeeCodes = new();
        public static PhieuCanKeToanViewModel Instance => instance ??= new PhieuCanKeToanViewModel();
        /// <summary>
        /// Toàn Công Ty , connStr = địa chỉ server
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public DataTable GetPhieuCanKeToansPV(DateTime fromDate, DateTime toDate, string? connStr = null)
        {
            try
            {
                var dao = new BravoModelV1.Dao.PhieuCanKeToan(connStr);
                return dao.Gets(fromDate, toDate);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Nhân Viên - Toàn Công Ty, connStr = địa chỉ server
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public DataTable GetPhieuCanKeToansNhanVienPV(DateTime fromDate, DateTime toDate, string? connStr = null)
        {
            try
            {
                var dao = new BravoModelV1.Dao.PhieuCanKeToan(connStr);
                return dao.GetsNhanVien(fromDate, toDate);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Định hình , connStr = địa chỉ server
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public DataTable GetTongHopSanPhamPhieuCanDinhHinhsPV(DateTime fromDate, DateTime toDate, string? connStr = null)
        {
            try
            {
                var dao = new BravoModelV1.Dao.PhieuCanDinhHinh(connStr);
                return dao.GetTongHopSanPhamsPV(fromDate, toDate);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Fillet , connStr = địa chỉ server
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public DataTable GetPhieuCanFilletsPV(DateTime fromDate, DateTime toDate, string? connStr = null)
        {
            try
            {
                var dao = new BravoModelV1.Dao.PhieuCanFillet(connStr);
                return dao.GetTongHopSanPhamsPV(fromDate, toDate);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Xếp Khuôn Trực Tiếp, Gian tiếp, kiểm định hình, sơ chế,kiểm sơ chế,phục vụ định hình,phụ fillet, bao tử ,phụ phẩm,lạng da connStr = địa chỉ server, khuVucId = 8 (Trực tiếp), 7(gián tiếp), 2(kiểm định hình) , 3(sơ chế) ,4(kiểm sơ chế), 5(Phụ vụ định hình), 6(phụ fillet),10(bao tử),11(phụ phẩm),12(lạng da)
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="khuVucId"></param>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public DataTable GetTongHopSanPhamPhieuCanKiemDinhhinhsPV(DateTime fromDate, DateTime toDate, int khuVucId, string? connStr = null)
        {
            try
            {
                var dao = new BravoModelV1.Dao.PhieuCanKiemDinhHinh(connStr);
                return dao.GetTongHopSanPhamsPV(fromDate, toDate, khuVucId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        

    }
}
