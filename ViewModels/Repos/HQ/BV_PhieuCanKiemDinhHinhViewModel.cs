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
    public partial class BV_PhieuCanKiemDinhHinhViewModel
    {
        private static BV_PhieuCanKiemDinhHinhViewModel instance;
        public static BV_PhieuCanKiemDinhHinhViewModel Instance => instance ??= new BV_PhieuCanKiemDinhHinhViewModel();
        public int Insert<T>(List<T> phieuCans, string? connStr = null)
        {
            try
            {
                var phieuCanDao = new Dao.Repos.HQ.PhieuCanKiemDinhHinh(connStr);
                var rows = phieuCanDao.Insert(phieuCans);
                return rows;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<BravoModelV1.Model.PhieuCanKiemDinhHinh> GetPhieuCanTinhLuongs(
            List<BravoModelV1.Model.SanLuongTinhLuongXepKhuon> sanLuongs,
            int khuVucId)
        {
            try
            {
                var phieuCans = new List<BravoModelV1.Model.PhieuCanKiemDinhHinh>();
                foreach (var item in sanLuongs)
                {
                    phieuCans.Add(
                        new BravoModelV1.Model.PhieuCanKiemDinhHinh()
                        {
                            CaLamViec = item.CaId,
                            MaNhanVien = item.MaNhanVien,
                            MaSanPham = item.BravoId,
                            Ngay = item.Ngay,
                            TenSanPham = item.TenCongViec,
                            TrongLuong = Math.Round(item.SanLuongHuong, 2),
                            _Status = 0,
                            KhuVuc = khuVucId.ToString("00")
                        });
                }

                return phieuCans;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
