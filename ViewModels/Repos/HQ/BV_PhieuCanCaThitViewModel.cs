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
    public partial class BV_PhieuCanCaThitViewModel
    {
        private static BV_PhieuCanCaThitViewModel instance;
        public static BV_PhieuCanCaThitViewModel Instance => instance ??= new BV_PhieuCanCaThitViewModel();
        public int Delete(DateTime dateTime, string? connStr = null)
        {
            var dao = new BravoModelV1.Dao.PhieuCanCaThit(connStr);
            return dao.Delete(dateTime);
        }
        public List<T> Gets<T>(DateTime dateTime, string? connStr = null)
        {
            try
            {
                var phieuCanDao = new BravoModelV1.Dao.PhieuCanCaThit(connStr);
                var rows = phieuCanDao.Gets<T>(dateTime);
                return rows;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public List<T> Gets<T>(string? connStr = null)
        {
            try
            {
                var phieuCanDao = new BravoModelV1.Dao.PhieuCanCaThit(connStr);
                var rows = phieuCanDao.Gets<T>();
                return rows;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public int Insert(List<BravoModelV1.Model.PhieuCanCaThit> phieuCans, string? connStr = null)
        {
            try
            {
                var phieuCanDao = new BravoModelV1.Dao.PhieuCanCaThit(connStr);
                var rows = phieuCanDao.Insert(phieuCans);
                return rows;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public int Insert<T>(List<T> phieuCans, string? connStr = null)
        {
            try
            {
                var phieuCanDao = new BravoModelV1.Dao.PhieuCanCaThit(connStr);
                var rows = phieuCanDao.Insert(phieuCans);
                return rows;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public int InsertBatch<T>(List<T> items, string? connStr = null)
        {
            try
            {
                var phieuCanDao = new BravoModelV1.Dao.PhieuCanCaThit(connStr);
                var rows = phieuCanDao.InsertBatch(items);
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
