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
    public partial class NhaCungCapNguyenLieuDonGiaVanChuyenViewModel : ObservableObject
    {
        private static NhaCungCapNguyenLieuDonGiaVanChuyenViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private NhaCungCapNguyenLieu_DonGiaVanChuyen? item;
        [ObservableProperty] private ObservableRangeCollection<NhaCungCapNguyenLieu_DonGiaVanChuyen> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private NhaCungCapNguyenLieu_DonGiaVanChuyen? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private NhaCungCapNguyenLieuDonGiaVanChuyenViewModel()
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

        public static NhaCungCapNguyenLieuDonGiaVanChuyenViewModel Instance => instance ??= new NhaCungCapNguyenLieuDonGiaVanChuyenViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public NhaCungCapNguyenLieu_DonGiaVanChuyen CopyItem(NhaCungCapNguyenLieu_DonGiaVanChuyen item)
        {
            return new NhaCungCapNguyenLieu_DonGiaVanChuyen
            {
                NgayApDung = item.NgayApDung,
                DonGia = item.DonGia,
                GhiChu = item.GhiChu,
                MaNhaCC = item.MaNhaCC
            };
        }
        public NhaCungCapNguyenLieu_DonGiaVanChuyen CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public NhaCungCapNguyenLieu_DonGiaVanChuyen CreateDefaultNew()
        {
            return new NhaCungCapNguyenLieu_DonGiaVanChuyen()
            {
                DonGia = 0,
                NgayApDung = DateTime.Now
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.NhaCungCapNguyenLieu_DonGiaVanChuyen();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(NhaCungCapNguyenLieu_DonGiaVanChuyen item)
        {
            try
            {
                if (Delete(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.MaNhaCC == item.MaNhaCC);
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
            var dao = new Dao.Repos.HQ.NhaCungCapNguyenLieu_DonGiaVanChuyen();
            return dao.Gets<T>();
        }
        public List<T> GetsFullField<T>(bool isServer = false)
        {
            var dao = new Dao.Repos.HQ.NhaCungCapNguyenLieu_DonGiaVanChuyen();
            return dao.GetsFullField<T>();
        }
        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.NhaCungCapNguyenLieu_DonGiaVanChuyen();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(NhaCungCapNguyenLieu_DonGiaVanChuyen item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<NhaCungCapNguyenLieu_DonGiaVanChuyen>();
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
        private bool IsItemPass(NhaCungCapNguyenLieu_DonGiaVanChuyen item)
        {
            return item != null && item.MaNhaCC != null && item.MaNhaCC.Trim() != "";
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

            var items = Gets<NhaCungCapNguyenLieu_DonGiaVanChuyen>();
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
        private void Reload_(ObservableRangeCollection<NhaCungCapNguyenLieu_DonGiaVanChuyen> obj)
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
            var dao = new Dao.Repos.HQ.NhaCungCapNguyenLieu_DonGiaVanChuyen();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(NhaCungCapNguyenLieu_DonGiaVanChuyen item)
        {
            try
            {
                if (Update(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.MaNhaCC == item.MaNhaCC);
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
                        var _item = Items.SingleOrDefault(x => x.MaNhaCC == Item.MaNhaCC);
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
        public NhaCungCapNguyenLieu_DonGiaVanChuyen? Find(string ma)
        {
            try
            {
                return Items.FirstOrDefault(x => x.MaNhaCC == ma);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Exists(NhaCungCapNguyenLieu_DonGiaVanChuyen item)
        {
            try
            {

                return Find(item.MaNhaCC) != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
    }
}
