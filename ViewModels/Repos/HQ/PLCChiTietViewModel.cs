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
    public partial class PLCChiTietViewModel : ObservableObject
    {
        private static PLCChiTietViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private PLCChiTiet? item;
        [ObservableProperty] private ObservableRangeCollection<PLCChiTiet> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private PLCChiTiet? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private PLCChiTietViewModel()
        {
            try
            {
                Reload();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }

        public static PLCChiTietViewModel Instance => instance ??= new PLCChiTietViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public PLCChiTiet CopyItem(PLCChiTiet item)
        {
            return new PLCChiTiet
            {
                
                Id = item.Id,
                MaCoi = item.MaCoi,
                MaXuong = item.MaXuong,
                PLCId = item.PLCId,
                RUN = item.RUN,
                STOP = item.STOP,
                TimeQuay = item.TimeQuay,
                HzQuay = item.HzQuay,
                HzRa = item.HzRa,
                HzQuayDef = item.HzQuayDef,
                HzRaDef = item.HzRaDef,
                State = item.State,
                TanSoQuay = item.TanSoQuay,
                TanSoRa = item.TanSoRa,
                ThoiGianQuay = item.ThoiGianQuay,
                TimeQuayDef = item.TimeQuayDef,
                TimeRaCoi = item.TimeRaCoi,
                TimeRun = item.TimeRun,
                TimeStop = item.TimeStop,
                INVERTER = item.INVERTER,
                RUNSTATUS = item.RUNSTATUS,
                IsConnected = item.IsConnected,
                PAUSE = item.PAUSE,
                RUNOUT = item.RUNOUT,
                NhanVienId = item.NhanVienId,
                TrongLuong = item.TrongLuong,
                IdMonitor = item.IdMonitor,
                IsPause = item.IsPause,
                IsPowerOn = item.IsPowerOn,
                IsRunOut = item.IsRunOut,
                LiteReports = item.LiteReports,
                Logs = item.Logs,
                MaChatLuong = item.MaChatLuong,
                

            };
        }
        public PLCChiTiet CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public PLCChiTiet CreateDefaultNew()
        {
           
            return new PLCChiTiet
            {
                
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PLCChiTiet();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(PLCChiTiet item)
        {
            try
            {
                if (Delete(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.Id == item.Id);
                        if (_item != null)
                        {
                            var index = Items.IndexOf(_item);
                            Items.RemoveAt(index);
                            //Items.Insert(index,item);
                        }
                    }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }

        private List<T> Gets<T>()
        {
            var dao = new Dao.Repos.HQ.PLCChiTiet();
            return dao.Gets<T>();
        }

        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PLCChiTiet();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(PLCChiTiet item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<PLCChiTiet>();
                    lock (Items)
                    {
                        Items.Add(item);
                    }
                    VmMessage.MessageBoxShow("Thực Hiện Xong", "Thông Báo", 0);
                    IsWindowItemShown = false;
                }
                else
                {
                    VmMessage.MessageBoxShow("Không thể thêm", "Thông Báo", 0);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }

        [RelayCommand()]
        private void Insert2_()
        {
            try
            {
                if (Insert(Item) > 0)
                {
                    Items.Insert(0, Item);
                    VmMessage.MessageBoxShow("Thực Hiện Xong", "Thông Báo", 0);
                }
            }
            catch (Exception e)
            {
                VmMessage.SetExceptionCommand.Execute(e);
                //throw;
            }
        }
        public bool IsVailSelectedItem => SelectedItem != null;
        private bool IsItemPass(PLCChiTiet item)
        {
            return item != null && item.MaCoi != null && item.MaCoi.Trim() !="" && item.MaXuong != null && item.MaXuong.Trim() != "" && item.PLCId != null && item.PLCId.Trim() != "" && item.RUN != null && item.RUN.Trim() != "" && item.STOP != null && item.STOP.Trim() != "" && item.TimeQuay != null && item.TimeQuay.Trim() !="" && item.HzQuay != null && item.HzQuay.Trim() !="" &&  item.HzRa != null && item.HzRa.Trim() !="";
        }
        //[RelayCommand]
        //private void ForceRaseCanExcute()
        //{
        //    try
        //    {
        //        Insert_Command.NotifyCanExecuteChanged();
        //        Delete_Command.NotifyCanExecuteChanged();
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        //throw;
        //    }
        //}
        public void Reload()
        {
            lock (Items)
            {
                Items.Clear();
            }

            var items = Gets<PLCChiTiet>();
            if (items.Any())
                lock (Items)
                {
                    try
                    {
                        //Items.AddRange(items);
                        foreach (var item in items)
                        {
                            Items.Add(item);
                        }

                      
                    }
                    catch (NotSupportedException e)
                    {

                    }

                }


        }

        [RelayCommand]
        private void Reload_(ObservableRangeCollection<PLCChiTiet> obj)
        {
            try
            {
                Reload();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }

        private int Update<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PLCChiTiet();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(PLCChiTiet item)
        {
            try
            {
                if (Update(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.Id == item.Id);
                        if (_item != null)
                        {
                            var index = Items.IndexOf(_item);
                            Items.RemoveAt(index);
                            Items.Insert(index, item);
                        }
                    }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }
        [RelayCommand()]
        private void Update2_()
        {
            try
            {
                if (Update(Item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.Id == Item.Id);
                        if (_item != null)
                        {
                            var index = Items.IndexOf(_item);
                            Items.RemoveAt(index);
                            Items.Insert(index, Item);
                            SelectedItem = Item;
                        }
                    }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }
        public PLCChiTiet? Find(string ma)
        {
            try
            {
                return Items.FirstOrDefault(x => x.Id == ma);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Exists(PLCChiTiet item)
        {
            try
            {

                return Find(item.Id) != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
