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
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;
using System.Windows.Input;
using Azure.Identity;
using System.Collections.Specialized;
using Vars.Hubs;
using Microsoft.Data.SqlClient;

namespace ViewModels.Repos.HQ
{
    public partial class PMS_DataRecordViewModel : ObservableObject
    {
        private static PMS_DataRecordViewModel instance;
        public static PMS_DataRecordViewModel Instance => instance ??= new PMS_DataRecordViewModel();
        public List<T> Gets<T>(DateTime dateTime, string khuVucId, string xuongId, string? connStr = null)
        {
            try
            {
                var dao = new BravoModelV1.Dao.PMS_DataRecord(connStr);
                return dao.Gets<T>(dateTime, khuVucId, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<string> GetSqlsInBatches(List<BravoModelV1.EF.PMS_DataRecord> items, string? connStr = null)
        {
            try
            {
                var dao = new BravoModelV1.Dao.PMS_DataRecord(connStr);
                return dao.GetSqlsInBatches(items);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<string> GetSqlsInUpdateBatches(List<BravoModelV1.EF.PMS_DataRecord> items, string? connStr = null)
        {
            try
            {
                var dao = new BravoModelV1.Dao.PMS_DataRecord(connStr);
                return dao.GetSqlsInUpdateBatches(items);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Delete(List<BravoModelV1.EF.PMS_DataRecord> items, string? connStr = null)
        {
            try
            {
                var dao = new BravoModelV1.Dao.PMS_DataRecord(connStr);
                return dao.Delete(items);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Delete(List<string> items, string? connStr = null)
        {
            try
            {
                var dao = new BravoModelV1.Dao.PMS_DataRecord(connStr);
                return dao.Delete(items);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Execute(string sql, string? connStr = null)
        {
            try
            {
                var dao = new BravoModelV1.Dao.PMS_DataRecord(connStr);
                return dao.ExecuteBatche(sql);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<BravoModelV1.EF.PMS_DataRecord> TPSoftConvert(
            List<PhieuCanTPDinhHinh> phieuCans,
            List<Models.Repos.AppModel.SanPhamBravo> sanPhams,
            List<NhanVienDaiThanh> nhanViens, string? connStr = null)
        {
            try
            {
                var items = new List<BravoModelV1.EF.PMS_DataRecord>();
                //foreach (var item in phieuCans)
                //{
                //    var _item = new BravoModelV1.EF.PMS_DataRecord()
                //    {
                //        CMND = 
                //    };
                //}
                //select new BravoModelV1.EF.PMS_DataRecord()
                //{
                //    ID =
                //                      $@"{p.MaMayCan}-{p.MaXuong}-K01-{p.Ngay.ToString("yyyy-MM-dd")}-{p.Gio.ToString("hh-mm-ss")}",
                //    TrongLuong = p.TrongLuongTra,
                //    CongDoanID = sp.Id,
                //    CMND = n.Tel == null ? "" : n.Tel,
                //    DateCreate = DateTime.Now,
                //    DateSync = DateTime.Now,
                //    Status = true,
                //    ThoiGian =
                //                      new DateTime(p.Ngay.Year, p.Ngay.Month, p.Ngay.Day, p.Gio.Hours, p.Gio.Minutes, p.Gio.Seconds)
                //}
                var _phieuCans = (from p in phieuCans
                                  from sp in sanPhams
                                  from n in nhanViens
                                  where p.MaNhanVien == n.MaNhanVien && p.MaThanhPham == sp.DaiThanhId
                                  select new { p.MaMayCan, p.MaXuong, p.Ngay, p.Gio, p.TrongLuongTra, sp.Id, CMND = n.Tel }).ToList();
                //return _phieuCans;
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        //public async Task ExecuteTPSoftActionAsync(DateTime dateTime, string maXuong)
        //{
        //    try
        //    {
        //        PhieuCanViewModel.Ins.IsBusy = true;
        //        PhieuCanViewModel.Ins.BusyString = "Khởi tạo TPSoft";
        //        await Task.Delay(50).ConfigureAwait(false);

        //        var phieuCans = PhieuCanTPDinhHinhViewModel.Instance.GetsTPSoft(dateTime, maXuong);
        //        var phieuCanTPSoftServer = Gets<BravoModelV1.EF.PMS_DataRecord>(dateTime, "01", maXuong);
        //        var idTPSofts = phieuCanTPSoftServer.Select(x => x.ID)
        //            .DefaultIfEmpty(string.Empty)
        //            .Distinct()
        //            .ToList();

        //        var insItems = phieuCans.Where(x => !idTPSofts.Contains(x.ID)).ToList();
        //        var udaItems = phieuCans.Where(x => idTPSofts.Contains(x.ID)).ToList();
        //        udaItems.ForEach(x => x.DateSync = new DateTime(1900, 01, 01, 0, 0, 0));

        //        if (insItems.Any())
        //        {
        //            var batches = GetSqlsInBatches(insItems);
        //            var rows = 0;
        //            PhieuCanViewModel.Ins.BusyString = "Bắt Đầu Thêm";
        //            await Task.Delay(50).ConfigureAwait(false);

        //            foreach (var batche in batches)
        //            {
        //                rows += Execute(batche);
        //                PhieuCanViewModel.Ins.BusyString = $@"Đã thực hiện {rows}/{insItems.Count}";
        //                await Task.Delay(50).ConfigureAwait(false);
        //            }
        //        }

        //        if (udaItems.Any())
        //        {
        //            var batches = GetSqlsInBatches(udaItems);
        //            var rows = 0;
        //            PhieuCanViewModel.Ins.BusyString = "Bắt Đầu Cập Nhật";
        //            await Task.Delay(50).ConfigureAwait(false);
        //            Delete(udaItems.Select(x => x.ID).ToList());

        //            foreach (var batche in batches)
        //            {
        //                rows += Execute(batche);
        //                PhieuCanViewModel.Ins.BusyString = $@"Đã thực hiện {rows}/{udaItems.Count}";
        //                await Task.Delay(50).ConfigureAwait(false);
        //            }
        //        }

        //        PhieuCanViewModel.Ins.BusyString = "Thực hiện xong!";
        //        await Task.Delay(500).ConfigureAwait(false);
        //    }
        //    catch (Exception exception)
        //    {
        //        MessageBox.Show(exception.Message);
        //    }
        //    finally
        //    {
        //        PhieuCanViewModel.Ins.IsBusy = false;
        //        PhieuCanViewModel.Ins.BusyString = "Đang tải...";
        //    }
        //}
    }
}
