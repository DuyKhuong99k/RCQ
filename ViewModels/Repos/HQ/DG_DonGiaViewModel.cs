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
    public partial class DG_DonGiaViewModel : ObservableObject
    {
        private static DG_DonGiaViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private DG_DonGia? item;
        [ObservableProperty] private ObservableRangeCollection<DG_DonGia> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private DG_DonGia? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private DG_DonGiaViewModel( )
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

        public static DG_DonGiaViewModel Instance => instance ??= new DG_DonGiaViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public DG_DonGia CopyItem(DG_DonGia item)
        {
            return new DG_DonGia
            {
                CreateBy = item.CreateBy,
                CreateDateTime = item.CreateDateTime,
                DanhGia = item.DanhGia,
                DinhMucDown = item.DinhMucDown,
                DinhMucUp = item.DinhMucUp,
                DonGia = item.DonGia,
                GhiChu = item.GhiChu,
                Gio = item.Gio,
                HeSo = item.HeSo,
                Id = item.Id,
                MaLoaiDonGia = item.MaLoaiDonGia,
                MaSanPham = item.MaSanPham,
                ModifiedBy = item.ModifiedBy,
                ModifiedDateTime = item.ModifiedDateTime,
                Ngay = item.Ngay,
                HeSoRot = item.HeSoRot,
                IsUsedHeSoRot = item.IsUsedHeSoRot,
                MaSizeDinhHinh = item.MaSizeDinhHinh,
                Range = item.Range,
                DonGiaGiaCong = item.DonGiaGiaCong,
                MaXepHang = item.MaXepHang,
                LoaiCan = item.LoaiCan,
                MaSizeFillet = item.MaSizeFillet,
                MaThanhPham = item.MaThanhPham
            };
        }
        public DG_DonGia CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public DG_DonGia CreateDefaultNew()
        {
            var maxId = Items.Where(x => int.TryParse(x.Id, out int rl)).Select(x => x.Id).DefaultIfEmpty("0")
                     .Max();
            var id = (int.Parse(maxId) + 1).ToString("0000000");
            return new DG_DonGia()
            {
                Id = id,
                //CreateBy = PMSMSharedv1.ViewModel.AppViewModel.Ins.UserName,
                //CreateDateTime = DateTime.Now,
                //DanhGia = true,
                //DinhMucDown = 0,
                //DinhMucUp = 0,
                //DonGia = 0,
                //Gio = DateTime.Now.TimeOfDay,
                //HeSo = 1,
                //ModifiedBy = PMSMSharedv1.ViewModel.AppViewModel.Ins.UserName,
                //ModifiedDateTime = DateTime.Now,
                //Ngay = PMSMSharedv1.ViewModel.AppViewModel.Ins.DateTimeNow,
                //HeSoRot = 1,
                //IsUsedHeSoRot = false,
                //Range = 0,
                //DonGiaGiaCong = 0,
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.DG_DonGia();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(DG_DonGia item)
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

        public List<T> Gets<T>()
        {
            var dao = new Dao.Repos.HQ.DG_DonGia();
            return dao.Gets<T>();
        }
        public List<T> Gets<T>(DateTime dateTime, string loaiDonGiaId, bool isLast = false, string loaiCan = "", string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.DG_DonGia(connStr);
                return dao.Gets<T>(dateTime, loaiDonGiaId, isLast, loaiCan);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetsAllList<T>(DateTime dateTime, bool isLast = false, string loaiCan ="")
        {
            try
            {
                var dao = new Dao.Repos.HQ.DG_DonGia();
                return dao.GetsAllList<T>(dateTime, isLast);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> Gets<T>(List<DateTime> dateTimes)
        {
            var items = new List<T>();
            foreach (var dateTime in dateTimes)
            {
                var _items = GetsAllList<T>(dateTime, true);
                if (_items.Any())
                {
                    
                    items.AddRange(_items);
                }
            }
            return items;
        }
        public List<T> Gets<T>(DateTime dateTime, bool isLast = false, string loaiCan = "", string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.DG_DonGia(connStr);
                return dao.Gets<T>(dateTime, isLast, loaiCan);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public T Gets<T>(DateTime dateTime, string sanPhamId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.DG_DonGia(connStr);
                return dao.Gets<T>(dateTime, sanPhamId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public T Gets2<T>(DateTime dateTime, string sanPhamId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.DG_DonGia(connStr);
                return dao.Gets<T>(dateTime, sanPhamId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.DG_DonGia();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(DG_DonGia item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<DG_DonGia>();
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
        private bool IsItemPass(DG_DonGia item)
        {
            return item != null && item.MaThanhPham != null && item.MaThanhPham.Trim() != "" &&
                   item.MaSizeFillet != null && item.MaSizeFillet.Trim() != "";
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

            var items = Gets<DG_DonGia>();
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
        private void Reload_(ObservableRangeCollection<DG_DonGia> obj)
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
            var dao = new Dao.Repos.HQ.DG_DonGia();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(DG_DonGia item)
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
        public DG_DonGia? Find(string id)
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

        public bool Exists(DG_DonGia item)
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
        public List<T> GetsFullFieldByYear<T>(string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.DG_DonGia(connStr);
            return dao.GetsFullFieldByYear<T>();
        }
    }
}
