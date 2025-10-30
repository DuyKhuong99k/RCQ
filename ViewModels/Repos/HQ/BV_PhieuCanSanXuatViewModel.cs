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
    public partial class BV_PhieuCanSanXuatViewModel

    {
        private static BV_PhieuCanSanXuatViewModel instance;
        public static BV_PhieuCanSanXuatViewModel Instance => instance ??= new BV_PhieuCanSanXuatViewModel();

        public Dictionary<string, string> GetCongDoans()
        {
            var items = new Dictionary<string, string>
            {
                { "TN", "Tiếp Nhận" },
                { "FL", "Fillet" },
                { "DH", "Định Hình" },
                { "XK", "Xếp Khuôn" },
                { "CXBN", "Phụ Phẩm - Cân Xuất Bán Ngoài" }
            };
            return items;
        }
        public List<T> Gets<T>(DateTime dateTime, string congDoanId, string? connStr = null)
        {
            var dao = new BravoModelV1.Dao.PhieuCanSanXuat(connStr);
            return dao.Gets<T>(dateTime, congDoanId);
        }
        public int InsertBatch<T>(List<T> items, string? connStr = null)
        {
            try
            {
                var phieuCanDao = new BravoModelV1.Dao.PhieuCanSanXuat(connStr);
                var rows = phieuCanDao.InsertBatch(items);
                return rows;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public int Delete(DateTime dateTime, string congDoanId, string? connStr = null)
        {
            var dao = new BravoModelV1.Dao.PhieuCanSanXuat(connStr);
            return dao.Delete(dateTime, congDoanId);
        }
    }
}
