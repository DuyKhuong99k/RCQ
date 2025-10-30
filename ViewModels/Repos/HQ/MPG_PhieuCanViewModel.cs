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
    public partial class MPG_PhieuCanViewModel : ObservableObject
    {
        private static MPG_PhieuCanViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private MPG_PhieuCan? item;
        [ObservableProperty] private ObservableRangeCollection<MPG_PhieuCan> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private MPG_PhieuCan? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private MPG_PhieuCanViewModel()
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

        public static MPG_PhieuCanViewModel Instance => instance ??= new MPG_PhieuCanViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public MPG_PhieuCan CopyItem(MPG_PhieuCan item)
        {
            return new MPG_PhieuCan
            {
                GhiChu = item.GhiChu,
                Gio = item.Gio,
                MaNhanVien = item.MaNhanVien,
                MaThe = item.MaThe,
                Ngay = item.Ngay,
                STT = item.STT,
                SuDung = item.SuDung,
                TrongLuong = item.TrongLuong,
                PhuTroi = item.PhuTroi,
                Block = item.Block,
                MaCongThucChiTiet = item.MaCongThucChiTiet,
                MaXuong = item.MaXuong,
                MayCan = item.MayCan,
                TrongLuongCongThuc = item.TrongLuongCongThuc,
            };
        }
        public MPG_PhieuCan CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public MPG_PhieuCan CreateDefaultNew()
        {
            
            return new MPG_PhieuCan
            {
                SuDung = true,
                Ngay = AppViewModel.Instance.DateTimeNow,
                Gio = DateTime.Now.TimeOfDay,
                MaXuong = XiNghiepViewModel.Instance.SelectedItem?.Ma
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.MPG_PhieuCan();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(MPG_PhieuCan item)
        {
            try
            {
                if (Delete(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.STT == item.STT);
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
            var dao = new Dao.Repos.HQ.MPG_PhieuCan();
            return dao.Gets<T>();
        }

        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.MPG_PhieuCan();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(MPG_PhieuCan item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<MPG_PhieuCan>();
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
        private bool IsItemPass(MPG_PhieuCan item)
        {
            return item != null &&
                 item.STT > 0 &&
                 item.Ngay != null &&
                 item.Gio != null &&
                 item.MayCan != null &&
                 item.MayCan.Trim() != string.Empty &&
                 item.TrongLuong >= 0 &&
                 item.MaNhanVien != null &&
                 item.MaNhanVien.Trim() != string.Empty &&
                 item.MaThe != null &&
                 item.MaXuong != null &&
                 item.MaXuong.Trim() != string.Empty &&
                 item.MaCongThucChiTiet != null &&
                 item.MaCongThucChiTiet.Trim() != string.Empty &&
                 item.PhuTroi >= 0 &&
                 item.TrongLuongCongThuc >= 0;
        }
        [RelayCommand]
        private void ForceRaseCanExcute()
        {
            try
            {
                Insert_Command.NotifyCanExecuteChanged();
                Delete_Command.NotifyCanExecuteChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }
        }
        public void Reload()
        {
            lock (Items)
            {
                Items.Clear();
            }

            var items = Gets<MPG_PhieuCan>();
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
        private void Reload_(ObservableRangeCollection<MPG_PhieuCan> obj)
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
            var dao = new Dao.Repos.HQ.MPG_PhieuCan();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(MPG_PhieuCan item)
        {
            try
            {
                if (Update(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.STT == item.STT);
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
                        var _item = Items.SingleOrDefault(x => x.STT == Item.STT);
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
        public MPG_PhieuCan? Find(int stt)
        {
            try
            {
                return Items.FirstOrDefault(x => x.STT == stt);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Exists(MPG_PhieuCan item)
        {
            try
            {

                return Find(item.STT) != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
