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

namespace ViewModels.Repos.HQ
{
    public partial class HQ_PhieuCanViewModel
    {
        private static HQ_PhieuCanViewModel instance;
        public static HQ_PhieuCanViewModel Instance => instance ??= new HQ_PhieuCanViewModel();
        private HQ_PhieuCanViewModel()
        {
            try
            {
                //Reload();

            }
            catch (Exception e)
            {
                
            }
        }
        public List<T> GetChiTiets<T>(DateTime fromDate, DateTime toDate ,string xuongId,string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCan(connStr);
            return dao.GetChiTiets<T>(fromDate,toDate,xuongId);
        }
        public List<T> GetTongHopNhanViens<T>(DateTime fromDate, DateTime toDate ,string xuongId,string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCan(connStr);
            return dao.GetTongHopNhanViens<T>(fromDate,toDate,xuongId);
        }
        public List<T> GetTongHopThanhPhams<T>(DateTime fromDate, DateTime toDate,string xuongId ,string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCan(connStr);
            return dao.GetTongHopThanhPhams<T>(fromDate,toDate,xuongId);
        }
        public List<T> GetChiTietXLPCs<T>(DateTime dateTime ,string xuongId,string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCan(connStr);
            return dao.GetChiTietXLPCs<T>(dateTime,xuongId);
        }
        public List<T> GetPhieuCanUpdateXLPCs<T>(DateTime dateTime ,string xuongId,string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCan(connStr);
            return dao.GetPhieuCanUpdateXLPCs<T>(dateTime,xuongId);
        }
        public List<T> GetPhieuCanDeleteXLPCs<T>(DateTime dateTime ,string xuongId,string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCan(connStr);
            return dao.GetPhieuCanDeleteXLPCs<T>(dateTime,xuongId);
        }

        public T? Get<T>(string id,string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCan(connStr);
            return dao.Get<T>(id);
        }

        public int Insert<T>(T item,string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCan(connStr);
            return dao.Insert(item);
        }
        public int Update<T>(T item, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCan(connStr);
            return dao.Update(item);
        }
        public int Delete<T>(T item, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCan(connStr);
            return dao.Delete(item);
        }
        //(Id, STT, NgayGio, MayCan, MaLo, MaSize, MaThanhPham, MaNhanVien, TrongLuongNhan, TrongLuongTra, TrongLuongTare, TheId, TheChucNang, Ngay, MaLoaiNguyenLieu);
        public Models.Repos.Models.HQ_PhieuCan CreateNew(string id,int stt,string malo,string maloainguyenlieu,string masize,string mathanhpham,string manhanvien,decimal trongluongnhan,decimal trongluongtra,decimal trongluongtare,string theid, string thechucnang,DateTime ngayGio,string maycan,string maxuong)
        {
            return new HQ_PhieuCan()
            {
                Id =id,
                MaLo = malo,
                MaLoaiNguyenLieu = maloainguyenlieu,
                MaSize = masize,
                MaThanhPham = mathanhpham,
                MayCan = maycan,
                TheId = theid,
                TheIdNhanVien = theid,
                NgayGio = ngayGio,
                ChiSanLuong = true,
                GhiChu = "",
                MaNhanVien = manhanvien,
                MaNhanVienBanKiem = "",
                MaNhanVienPhucVu = "",
                MaXuong = maxuong,
                Ngay = DateOnly.FromDateTime(ngayGio),
                STT = stt,
                Status = 1,
                TrongLuong = trongluongnhan,
                TrongLuongTare = trongluongtare,
                
            };

        }
    }
}
