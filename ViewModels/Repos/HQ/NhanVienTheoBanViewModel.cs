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
    public partial class NhanVienTheoBanViewModel : ObservableObject
    {
        private static NhanVienTheoBanViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private NhanVienTheoBan? item;
        [ObservableProperty] private ObservableRangeCollection<NhanVienTheoBan> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private NhanVienTheoBan? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private NhanVienTheoBanViewModel()
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

        public static NhanVienTheoBanViewModel Instance => instance ??= new NhanVienTheoBanViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public NhanVienTheoBan CopyItem(NhanVienTheoBan item)
        {
            return new NhanVienTheoBan
            {
                Id = item.Id,
                NgayGio = item.NgayGio,
                MaNhanVien = item.MaNhanVien,
                MaBan = item.MaBan,
                MaKhuVuc = item.MaKhuVuc,
                MaXuong = item.MaXuong,
                PCName = item.PCName,
                UserName = item.UserName,
                IsDone = item.IsDone
            };
        }
        public NhanVienTheoBan CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public NhanVienTheoBan CreateDefaultNew()
        {
            var maxId = Items.Where(x => int.TryParse(x.Id, out var rl))
                .Select(x => int.Parse(x.Id))
                .DefaultIfEmpty(0)
                .Max();
            var id = $"{(maxId + 1).ToString("0000000")}";
            //return new T_ChiTietVoXo { Id = id };

            return new NhanVienTheoBan
            {
                Id = id,
                PCName = AppViewModel.Instance.PCName,
                //UserName = AppViewModel.Instance.UserName,
                IsDone = false
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.NhanVienTheoBan();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(NhanVienTheoBan item)
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
            var dao = new Dao.Repos.HQ.NhanVienTheoBan();
            return dao.Gets<T>();
        }
        public string? GetBanId(DateTime dateTime, string xuongId, string nhanVienId)
        {
            try
            {
                var item = Items.Where(x => x.MaNhanVien == nhanVienId && x.MaXuong == xuongId).MaxBy(x => x.NgayGio);

                return item?.MaBan;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public string? GetBanId(string nhanVienId, string xuongId)
        {
            try
            {
                var item = Items.Where(x => x.MaNhanVien == nhanVienId && x.MaXuong == xuongId).MaxBy(x => x.NgayGio);

                return item?.MaBan;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.NhanVienTheoBan();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(NhanVienTheoBan item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<NhanVienTheoBan>();
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
        private bool IsItemPass(NhanVienTheoBan item)
        {
            return item != null && item.Id != null && item.Id.Trim() != "";
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

            var items = Gets<NhanVienTheoBan>();
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
        private void Reload_(ObservableRangeCollection<NhanVienTheoBan> obj)
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
            var dao = new Dao.Repos.HQ.NhanVienTheoBan();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(NhanVienTheoBan item)
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
        public NhanVienTheoBan? Find(string ma)
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

        public bool Exists(NhanVienTheoBan item)
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
        public List<T> GetsFullField<T>(string maBan,string xuong,string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.NhanVienTheoBan(connStr);
            return dao.GetNhanVienTheoBanFullFileds<T>(maBan,xuong);
        }
        public List<T> GetNhanVienTheoBansMoiNhat<T>(string maBan,string xuong,string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.NhanVienTheoBan(connStr);
            return dao.GetNhanVienTheoBansMoiNhat<T>(maBan,xuong);
        }
    }
}
