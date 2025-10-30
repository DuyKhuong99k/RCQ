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
    public partial class TrangThaiNhanVienTamThoiViewModel : ObservableObject
    {
        private static TrangThaiNhanVienTamThoiViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private TrangThaiNhanVienTamThoi? item;
        [ObservableProperty] private ObservableRangeCollection<TrangThaiNhanVienTamThoi> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private TrangThaiNhanVienTamThoi? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private TrangThaiNhanVienTamThoiViewModel()
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

        public static TrangThaiNhanVienTamThoiViewModel Instance => instance ??= new TrangThaiNhanVienTamThoiViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public TrangThaiNhanVienTamThoi CopyItem(TrangThaiNhanVienTamThoi item)
        {
            return new TrangThaiNhanVienTamThoi
            {
                Gio = item.Gio,
                IsPhucVu = item.IsPhucVu,
                MaNhanVien = item.MaNhanVien,
                MaXuong = item.MaXuong,
                Ngay = item.Ngay,
                STT = item.STT,
                KhuVuc = item.KhuVuc
            };
        }
        public TrangThaiNhanVienTamThoi CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public TrangThaiNhanVienTamThoi CreateDefaultNew()
        {
           
            return new TrangThaiNhanVienTamThoi
            {
                Gio = DateTime.Now.TimeOfDay,
                IsPhucVu = false,
                //KhuVuc = KhuVucWorking,
                MaXuong = XiNghiepViewModel.Instance.SelectedItem.Ma,
                Ngay = AppViewModel.Instance.DateTimeNow.Date,
                STT =
                        Items.Where(
                                x => x.MaXuong == XiNghiepViewModel.Instance.SelectedItem?.Ma)
                            .Select(x => x.STT)
                            .DefaultIfEmpty(0)
                            .Max() +
                        1,
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.TrangThaiNhanVienTamThoi();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(TrangThaiNhanVienTamThoi item)
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
            var dao = new Dao.Repos.HQ.TrangThaiNhanVienTamThoi();
            return dao.Gets<T>();
        }

        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.TrangThaiNhanVienTamThoi();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(TrangThaiNhanVienTamThoi item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<TrangThaiNhanVienTamThoi>();
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
        private bool IsItemPass(TrangThaiNhanVienTamThoi item)
        {
            return item != null &&
                 item.STT > 0 &&
                 item.Ngay != null &&
                 item.MaXuong != null &&
                 item.MaXuong.Trim() != string.Empty &&
                 item.MaNhanVien != null &&
                 item.MaNhanVien.Trim() != string.Empty &&
                 item.Gio != null &&
                 item.KhuVuc != null;
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

            var items = Gets<TrangThaiNhanVienTamThoi>();
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
        private void Reload_(ObservableRangeCollection<TrangThaiNhanVienTamThoi> obj)
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
            var dao = new Dao.Repos.HQ.TrangThaiNhanVienTamThoi();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(TrangThaiNhanVienTamThoi item)
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
        public TrangThaiNhanVienTamThoi? Find(int stt,DateTime dateTime,string xuongId,string khuVucId)
        {
            try
            {
                return Items.FirstOrDefault(x => x.STT == stt && x.Ngay.Date == dateTime.Date && x.MaXuong == xuongId && x.KhuVuc == khuVucId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Exists(TrangThaiNhanVienTamThoi item)
        {
            try
            {

                return Find(item.STT,item.Ngay,item.MaXuong,item.KhuVuc) != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Get danh sách nhân viên trạng thái tạm thời
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dateTime"></param>
        /// <param name="isServer"></param>
        /// <returns></returns>
        public List<T> Gets<T>(DateTime dateTime)
        {
            try
            {
                var dao = new Dao.Repos.HQ.TrangThaiNhanVienTamThoi();
                return dao.Gets<T>(dateTime);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
