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
    public partial class ThanhPhamFillet_HanMucTrongLuongViewModel : ObservableObject
    {
        private static ThanhPhamFillet_HanMucTrongLuongViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private MaThanhPhamFillet_HanMucTrongLuong? item;
        [ObservableProperty] private ObservableRangeCollection<MaThanhPhamFillet_HanMucTrongLuong> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private MaThanhPhamFillet_HanMucTrongLuong? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private ThanhPhamFillet_HanMucTrongLuongViewModel()
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

        public static ThanhPhamFillet_HanMucTrongLuongViewModel Instance => instance ??= new ThanhPhamFillet_HanMucTrongLuongViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public MaThanhPhamFillet_HanMucTrongLuong CopyItem(MaThanhPhamFillet_HanMucTrongLuong item)
        {
            return new MaThanhPhamFillet_HanMucTrongLuong
            {
                STT = item.STT,
                Gio = item.Gio,
                MaLo = item.MaLo,
                MaThanhPham = item.MaThanhPham,
                MaXuong = item.MaXuong,
                Ngay = item.Ngay,
                TrongLuong = item.TrongLuong
            };
        }
        public MaThanhPhamFillet_HanMucTrongLuong CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public MaThanhPhamFillet_HanMucTrongLuong CreateDefaultNew(DateTime dateTime, string xuongId)
        {
            var maxId = Items.Where(x => x.Ngay.Date == dateTime.Date && x.MaXuong == xuongId)
                    .Select(x => x.STT)
                    .DefaultIfEmpty(0)
                    .Max();
            var id = maxId + 1;
            return new MaThanhPhamFillet_HanMucTrongLuong()
            {
                STT = id,
                Ngay = dateTime.Date,
                MaXuong = xuongId,
                MaThanhPham =
                    ThanhPhamDinhHinhViewModel.Instance.Items
                        .Where(x => x.SuDung == true)
                        .FirstOrDefault()?.Ma,
                Gio = DateTime.Now.TimeOfDay
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.MaThanhPhamFillet_HanMucTrongLuong();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(MaThanhPhamFillet_HanMucTrongLuong item)
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
            var dao = new Dao.Repos.HQ.MaThanhPhamFillet_HanMucTrongLuong();
            return dao.Gets<T>();
        }

        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.MaThanhPhamFillet_HanMucTrongLuong();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(MaThanhPhamFillet_HanMucTrongLuong item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<MaThanhPhamFillet_HanMucTrongLuong>();
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
        private bool IsItemPass(MaThanhPhamFillet_HanMucTrongLuong item)
        {
            return item.STT > 0 &&
                 item.Ngay != null &&
                 item.Gio != null &&
                 item.MaLo != null &&
                 item.MaLo.Trim() != string.Empty &&
                 item.MaThanhPham != null &&
                 item.MaThanhPham.Trim() != string.Empty &&
                 item.MaXuong != null &&
                 item.MaXuong.Trim() != string.Empty &&
                 item.TrongLuong >= 0;
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

            var items = Gets<MaThanhPhamFillet_HanMucTrongLuong>();
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
        private void Reload_(ObservableRangeCollection<MaThanhPhamFillet_HanMucTrongLuong> obj)
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
            var dao = new Dao.Repos.HQ.MaThanhPhamFillet_HanMucTrongLuong();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(MaThanhPhamFillet_HanMucTrongLuong item)
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
        public MaThanhPhamFillet_HanMucTrongLuong? Find(int ma)
        {
            try
            {
                return Items.FirstOrDefault(x => x.STT == ma);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Exists(MaThanhPhamFillet_HanMucTrongLuong item)
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
        public List<T> GetsFullField<T>(DateTime ngay, string xuong,string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.MaThanhPhamFillet_HanMucTrongLuong(connStr);
            return dao.GetsFullField<T>(ngay,xuong);
        }
    }
}
