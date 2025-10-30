using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AppViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Models.Repos.Models;
using MvvmHelpers;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;
using Security.Crypt;

namespace ViewModels.Repos.HQ
{
    public partial class NguoiDungViewModel  : ObservableObject
    {
        private static NguoiDungViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private NguoiDung? item;
        [ObservableProperty] private ObservableRangeCollection<NguoiDung> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private NguoiDung? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private NguoiDungViewModel()
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
        
        public static NguoiDungViewModel Instance => instance ??= new NguoiDungViewModel();
        public bool IsVailSelectedItem => SelectedItem != null;
        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;
        public List<T> Gets<T>(string? connStr = null,bool isServer = false)
        {
            
            var dao = new Dao.Repos.HQ.NguoiDung(connStr);
            return dao.Gets<T>();
        }
        public int? GetId(string  userName,string? connStr = null)
        {
            
            var dao = new Dao.Repos.HQ.NguoiDung(connStr);
            return dao.GetId(userName);
        }

        public List<NguoiDung> GetByUserName(string userName,string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.NguoiDung(connStr);
            return dao.GetByUserName<NguoiDung>(userName);
        }

        
    }
}
