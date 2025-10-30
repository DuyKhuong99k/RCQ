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
    public partial class BV_PhieuCanCaGiongViewModel
    {
        private static BV_PhieuCanCaGiongViewModel instance;
        public static BV_PhieuCanCaGiongViewModel Instance => instance ??= new BV_PhieuCanCaGiongViewModel();
        public List<T> Gets<T>(DateTime dateTime, string? connStr = null)
        {
            var dao = new BravoModelV1.Dao.PhieuCanCaGiong(connStr);
            return dao.Gets<T>(dateTime);
        }
        public int Delete(DateTime dateTime, string? connStr = null)
        {
             var dao = new BravoModelV1.Dao.PhieuCanCaGiong(connStr);
            return dao.Delete(dateTime);
        }
        public int InsertBatch<T>(List<T> items, string? connStr = null)
        {
            try
            {
                var dao = new BravoModelV1.Dao.PhieuCanCaGiong(connStr);
                var rows = dao.InsertBatch(items);
                return rows;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
