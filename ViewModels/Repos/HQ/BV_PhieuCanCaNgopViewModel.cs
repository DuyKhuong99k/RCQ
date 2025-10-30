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
    public partial class BV_PhieuCanCaNgopViewModel
    {
        private static BV_PhieuCanCaNgopViewModel instance;
        public static BV_PhieuCanCaNgopViewModel Instance => instance ??= new BV_PhieuCanCaNgopViewModel();
        public int Delete(DateTime dateTime, string? connStr = null)
        {
            var dao = new BravoModelV1.Dao.PhieuCanCaNgop(connStr);
            return dao.Delete(dateTime);
        }
        public List<T> Gets<T>(DateTime dateTime, string? connStr = null)
        {
            var dao = new BravoModelV1.Dao.PhieuCanCaNgop(connStr);
            return dao.Gets<T>(dateTime);
        }

        public List<T> Gets<T>(string? connStr = null)
        {
            var dao = new BravoModelV1.Dao.PhieuCanCaNgop(connStr);
            return dao.Gets<T>();
        }
        public int Insert(List<BravoModelV1.Model.PhieuCanCaNgop> phieuCans,string? connStr = null)
        {
            try
            {
                var phieuCanDao = new BravoModelV1.Dao.PhieuCanCaNgop(connStr);
                var rows = phieuCanDao.Insert(phieuCans);
                return rows;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public int Insert<T>(List<T> phieuCans,string? connStr = null)
        {
            try
            {
                var phieuCanDao = new BravoModelV1.Dao.PhieuCanCaNgop(connStr);
                var rows = phieuCanDao.Insert(phieuCans);
                return rows;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public int InsertBatch<T>(List<T> items,string? connStr = null)
        {
            try
            {
                var phieuCanDao = new BravoModelV1.Dao.PhieuCanCaNgop(connStr);
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
