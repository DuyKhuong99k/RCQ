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
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;
using System.Windows.Input;
using Azure.Identity;
using System.Collections.Specialized;

namespace ViewModels.Repos.HQ
{
    public partial class BV_PhieuCanBotCaViewModel : ObservableObject
    {
        private static BV_PhieuCanBotCaViewModel instance;
        public static BV_PhieuCanBotCaViewModel Instance => instance ??= new BV_PhieuCanBotCaViewModel();

        public int Delete(DateTime dateTime, string? connStr = null)
        {
            var dao = new BravoModelV1.Dao.PhieuCanBotCa(connStr);
            return dao.Delete(dateTime);
        }
        public List<T> Gets<T>(string? connStr = null)
        {
            try
            {
                var dao = new BravoModelV1.Dao.PhieuCanBotCa(connStr);
                return dao.Gets<T>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public List<T> Gets<T>(DateTime dateTime, string? connStr = null)
        {
            try
            {
                var dao = new BravoModelV1.Dao.PhieuCanBotCa(connStr);
                return dao.Gets<T>(dateTime);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public int Insert(List<BravoModelV1.Model.PhieuCanBotCa> phieuCans, string? connStr = null)
        {
            try
            {
                var phieuCanDao = new BravoModelV1.Dao.PhieuCanBotCa(connStr);
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
                var phieuCanDao = new BravoModelV1.Dao.PhieuCanBotCa(connStr);
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
                var phieuCanDao = new BravoModelV1.Dao.PhieuCanBotCa(connStr);
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


