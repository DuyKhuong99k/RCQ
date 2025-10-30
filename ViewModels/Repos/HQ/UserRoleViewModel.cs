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
    public partial class UserRoleViewModel : ObservableObject
    {
        private static UserRoleViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private UserRole? item;
        [ObservableProperty] private ObservableRangeCollection<UserRole> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private UserRole? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private UserRoleViewModel()
        {
            try
            {
                //Reload();
                

            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                ////throw;
                //VmMessage.SetExceptionCommand.Execute(e);
            }
        }
        
        public static UserRoleViewModel Instance => instance ??= new UserRoleViewModel();
        public bool IsVailSelectedItem => SelectedItem != null;
        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;
        public List<T> GetsFullField<T>(string? connStr = null,bool isServer = false)
        {
            
            var dao = new Dao.Repos.HQ.UserRole(connStr);
            return dao.GetsFullField<T>();
        }
        public List<T> GetsFullField<T>(int userId,string? connStr = null)
        {
            
            var dao = new Dao.Repos.HQ.UserRole(connStr);
            return dao.GetsFullField<T>(userId);
        }
        public List<T> GetsFullFieldById<T>(int userRoleId,string? connStr = null)
        {
            
            var dao = new Dao.Repos.HQ.UserRole(connStr);
            return dao.GetsFullFieldById<T>(userRoleId);
        }
    }
}
