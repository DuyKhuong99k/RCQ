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
    public partial class BV_PhieuCanFilletViewModel
    {
        private static BV_PhieuCanFilletViewModel instance;
        public static BV_PhieuCanFilletViewModel Instance => instance ??= new BV_PhieuCanFilletViewModel();
        public int Insert<T>(List<T> items)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanTPFillet();
                return dao.InsertFillet(items);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
