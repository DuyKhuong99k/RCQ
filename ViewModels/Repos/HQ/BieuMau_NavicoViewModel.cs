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
    public partial class BieuMau_NavicoViewModel :ObservableObject
    {
        private static BieuMau_NavicoViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private BieuMau_Navico? item;
        [ObservableProperty] private ObservableRangeCollection<BieuMau_Navico> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private BieuMau_Navico? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private BieuMau_NavicoViewModel()
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

        public static BieuMau_NavicoViewModel Instance => instance ??= new BieuMau_NavicoViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public BieuMau_Navico CopyItem(BieuMau_Navico item)
        {
            return new BieuMau_Navico
            {
                Id = item.Id,
                Ngay = item.Ngay,
                MaNhanVien = item.MaNhanVien,
                BoPhan = item.BoPhan,
                MaCongDoan = item.MaCongDoan,
                NguyenLieu_Ngay = item.NguyenLieu_Ngay,
                NguyenLieu_Dem = item.NguyenLieu_Dem,
                ThanhPham_Ngay = item.ThanhPham_Ngay,
                ThanhPham_Dem = item.ThanhPham_Dem,
                MaNhanVienCan = item.MaNhanVienCan,
                MaNhaMay = item.MaNhaMay,
                ThoiDiemGhiNhanSanLuong = item.ThoiDiemGhiNhanSanLuong,
                MaXuong = item.MaXuong,
                MaKV = item.MaKV,
            };
        }
        public BieuMau_Navico CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public BieuMau_Navico CreateDefaultNew()
        {
            var maxId = Items.Where(x => int.TryParse(x.Id, out var rl))
                   .Select(x => int.Parse(x.Id))
                   .DefaultIfEmpty(0)
                   .Max();
            var id = $"{(maxId + 1).ToString()}";
            return new BieuMau_Navico
            {
                Id = id
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.BieuMau_Navico();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(BieuMau_Navico item)
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
            var dao = new Dao.Repos.HQ.BieuMau_Navico();
            return dao.Gets<T>();
        }

        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.BieuMau_Navico();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(BieuMau_Navico item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<BieuMau_Navico>();
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
        private bool IsItemPass(BieuMau_Navico item)
        {
            return item != null && item.MaCongDoan != null && item.MaCongDoan.Trim() != "" && item.BoPhan != null && item.BoPhan.Trim() != "" &&
                item.MaNhaMay != null && item.MaNhaMay.Trim() != "" && item.MaNhanVien != null && item.MaNhanVien.Trim() != "" && item.MaNhanVienCan != null && item.MaNhanVienCan.Trim() != "" &&
                item.MaXuong != null && item.MaXuong.Trim() != "" && item.MaKV != null && item.MaKV.Trim() != "" && item.ThoiDiemGhiNhanSanLuong != null;


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

            var items = Gets<BieuMau_Navico>();
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
        private void Reload_(ObservableRangeCollection<BieuMau_Navico> obj)
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
            var dao = new Dao.Repos.HQ.BieuMau_Navico();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(BieuMau_Navico item)
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
        public BieuMau_Navico? Find(string id)
        {
            try
            {
                return Items.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Exists(string id)
        {
            try
            {

                return Find(id) != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}

