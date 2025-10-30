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
    public partial class NguoiDung_QuyenViewModel : ObservableObject
    {
        private static NguoiDung_QuyenViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private NguoiDung_Quyen? item;
        [ObservableProperty] private ObservableRangeCollection<NguoiDung_Quyen> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private NguoiDung_Quyen? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private NguoiDung_QuyenViewModel()
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

        public static NguoiDung_QuyenViewModel Instance => instance ??= new NguoiDung_QuyenViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public NguoiDung_Quyen CopyItem(NguoiDung_Quyen item)
        {
            return new NguoiDung_Quyen
            {
                CaiDat = item.CaiDat,
                TenNguoiDung = item.TenNguoiDung,
                ChamGio = item.ChamGio,
                KetChuyen = item.KetChuyen,
                Loai = item.Loai,
                MaXuong = item.MaXuong,
                NhomNguoiDung = item.NhomNguoiDung,
                Sua = item.Sua,
                Them = item.Them,
                Xem = item.Xem,
                Xoa = item.Xoa
            };
        }
        public NguoiDung_Quyen CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public NguoiDung_Quyen CreateDefaultNew()
        {
           
            return new NguoiDung_Quyen
            {
                Xoa = false,
                CaiDat = false,
                Xem = false,
                Them = false,
                Sua = false,
                Loai = 0,
                KetChuyen = false,
                ChamGio = false,
                MaXuong = XiNghiepViewModel.Instance.SelectedItem?.Ma
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.NguoiDung_Quyen();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(NguoiDung_Quyen item)
        {
            try
            {
                if (Delete(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.TenNguoiDung == item.TenNguoiDung);
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
            var dao = new Dao.Repos.HQ.NguoiDung_Quyen();
            return dao.Gets<T>();
        }

        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.NguoiDung_Quyen();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(NguoiDung_Quyen item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<NguoiDung_Quyen>();
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
        private bool IsItemPass(NguoiDung_Quyen item)
        {
            return item.TenNguoiDung != null &&
                 item.TenNguoiDung.Trim() != string.Empty &&
                 item.NhomNguoiDung != null &&
                 item.MaXuong != null &&
                 item.MaXuong.Trim() != string.Empty;
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

            var items = Gets<NguoiDung_Quyen>();
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
        private void Reload_(ObservableRangeCollection<NguoiDung_Quyen> obj)
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
            var dao = new Dao.Repos.HQ.NguoiDung_Quyen();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(NguoiDung_Quyen item)
        {
            try
            {
                if (Update(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.TenNguoiDung == item.TenNguoiDung);
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
                        var _item = Items.SingleOrDefault(x => x.TenNguoiDung == Item.TenNguoiDung);
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
        public NguoiDung_Quyen? Find(string ma)
        {
            try
            {
                return Items.FirstOrDefault(x => x.TenNguoiDung == ma);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Exists(NguoiDung_Quyen item)
        {
            try
            {

                return Find(item.TenNguoiDung) != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
