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
    public partial class BV_PhieuCanDinhHinhViewModel
    {
        private static BV_PhieuCanDinhHinhViewModel instance;
        public static BV_PhieuCanDinhHinhViewModel Instance => instance ??= new BV_PhieuCanDinhHinhViewModel();
        public int Insert(List<PhieuCanDinhHinh> phieuCanDinhHinhs, string? connStr = null)
        {
            try
            {
                var phieuCanDao = new Dao.Repos.HQ.PhieuCanDinhHinh(connStr);
                var rows = phieuCanDao.Insert(phieuCanDinhHinhs);
                return rows;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public int Delete(DateTime dateTime, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanDinhHinh(connStr);
                return dao.Delete(dateTime);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
