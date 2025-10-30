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
    public partial class NhomXepKhuonViewModel
    {
        private static NhomXepKhuonViewModel instance;
        public static NhomXepKhuonViewModel Instance => instance ??= new NhomXepKhuonViewModel();
        public List<MaNhanVienTheoNhomXepKhuon> GetNhanVienNhomXepKhuons(DateTime dateTime,string? connStr = null)
        {
            try
            {
                var dao =  new Dao.Repos.HQ.MaNhanVienTheoNhomXepKhuon(connStr);
                return dao.Gets(dateTime);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
