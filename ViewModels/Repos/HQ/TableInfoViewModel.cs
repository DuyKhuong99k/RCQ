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
    public class TableInfoViewModel
    {
        private static TableInfoViewModel instance;
        private readonly SynchronizationContext synchronizationContext;
        private TableInfoViewModel()
        {
            try
            {
               // Reload();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }

        public static TableInfoViewModel Instance => instance ??= new TableInfoViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;
        public List<T> GetsTableInfo<T>(string tableInfo,string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.TableInfo(connStr);
            return dao.GetsTableInfo<T>(tableInfo);
        }
    }
}
