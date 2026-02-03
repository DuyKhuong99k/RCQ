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
using System.Linq;
using ToolsEx;

namespace ViewModels.Repos.HQ
{
    public class HQ_PhieuCanNhapNguyenLieuViewModel
    {
        private static HQ_PhieuCanNhapNguyenLieuViewModel instance;
        public static HQ_PhieuCanNhapNguyenLieuViewModel Instance => instance ??= new HQ_PhieuCanNhapNguyenLieuViewModel();
        private HQ_PhieuCanNhapNguyenLieuViewModel()
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<T> GetChiTietPhieuCanNhapNguyenLieus<T>(DateTime fromDate, DateTime toDate, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCanNhapNguyenLieu(connStr);
            return dao.GetChiTietPhieuCanNhapNguyenLieus<T>(fromDate, toDate, xuongId);
        }
        public List<T> GetChiTietPhieuCanNhapNguyenLieus<T>(DateTime dateTime, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCanNhapNguyenLieu(connStr);
            return dao.GetChiTietPhieuCanNhapNguyenLieus<T>(dateTime,xuongId);
        }
        public List<T> GetTongHopSanPhamPhieuCanNguyenLieus<T>(DateTime fromDate, DateTime toDate, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.HQ_PhieuCanNhapNguyenLieu(connStr);
            return dao.GetTongHopSanPhamPhieuCanNguyenLieus<T>(fromDate, toDate, xuongId);
        }

    }
}
