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
using System.Data;
namespace ViewModels.Repos.HQ
{
    public partial class PhieuCanCaGiongViewModel : ObservableObject
    {
        private static PhieuCanCaGiongViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private PhieuCanCaGiongVungNuoiDaiThanhSide? item;
        [ObservableProperty] private ObservableRangeCollection<PhieuCanCaGiongVungNuoiDaiThanhSide> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private PhieuCanCaGiongVungNuoiDaiThanhSide? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private PhieuCanCaGiongViewModel()
        {
            try
            {
                //Reload();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }

        public static PhieuCanCaGiongViewModel Instance => instance ??= new PhieuCanCaGiongViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public PhieuCanCaGiongVungNuoiDaiThanhSide CopyItem(PhieuCanCaGiongVungNuoiDaiThanhSide item)
        {
            return new PhieuCanCaGiongVungNuoiDaiThanhSide
            {
                STT = item.STT,
                Ngay = item.Ngay,
                MaMayCan = item.MaMayCan,
                Gio = item.Gio,
                MaUserCan = item.MaUserCan,
                GhiChu = item.GhiChu,
                MaLoaiCaDaiThanhId = item.MaLoaiCaDaiThanhId,
                MaGhe = item.MaGhe,
                TenAo = item.TenAo,
                TenChuAo = item.TenChuAo,
                TenCongDoan = item.TenCongDoan,
                TenThongKeDauAo = item.TenThongKeDauAo,
                TrongLuongTare = item.TrongLuongTare,
                TrongLuong = item.TrongLuong,
                BiosId = item.BiosId,
                GioTai = item.GioTai,
                NgayTai = item.NgayTai,
                TenLoaiCa = item.TenLoaiCa
            };
        }
        public PhieuCanCaGiongVungNuoiDaiThanhSide CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public PhieuCanCaGiongVungNuoiDaiThanhSide CreateDefaultNew()
        {

            return new PhieuCanCaGiongVungNuoiDaiThanhSide
            {
                Ngay = AppViewModel.Instance.DateTimeNow.Date,
                Gio = DateTime.Now.TimeOfDay
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PhieuCanCaGiongVungNuoiDaiThanhSide();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(PhieuCanCaGiongVungNuoiDaiThanhSide item)
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
            var dao = new Dao.Repos.HQ.PhieuCanCaGiongVungNuoiDaiThanhSide();
            return dao.Gets<T>();
        }
         public List<T> Gets<T>(DateTime dateTime)
        {
            var dao = new Dao.Repos.HQ.PhieuCanCaGiongVungNuoiDaiThanhSide();
            return dao.Gets<T>(dateTime);
        }
        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PhieuCanCaGiongVungNuoiDaiThanhSide();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(PhieuCanCaGiongVungNuoiDaiThanhSide item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<PhieuCanCaGiongVungNuoiDaiThanhSide>();
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
        private bool IsItemPass(PhieuCanCaGiongVungNuoiDaiThanhSide item)
        {
            return item != null;
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

            var items = Gets<PhieuCanCaGiongVungNuoiDaiThanhSide>();
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
        private void Reload_(ObservableRangeCollection<PhieuCanCaGiongVungNuoiDaiThanhSide> obj)
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
            var dao = new Dao.Repos.HQ.PhieuCanCaGiongVungNuoiDaiThanhSide();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(PhieuCanCaGiongVungNuoiDaiThanhSide item)
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
        #region Xử Lý Phiếu Cân
        public List<T> GetPhieuCan<T>(DateTime dateTime, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanCaGiongVungNuoiDaiThanhSide(connStr);
            return dao.GetPhieuCan<T>(dateTime);
        }
        #endregion
        #region Báo Cáo
        public List<T> GetChiTiets<T>(DateTime fromDate, DateTime toDate)
        {
            var dao = new Dao.Repos.HQ.PhieuCanCaGiongVungNuoiDaiThanhSide();
            return dao.GetChiTiets<T>(fromDate,toDate);
        }
        public List<T> GetTongHops<T>(DateTime fromDate, DateTime toDate)
        {
            var dao = new Dao.Repos.HQ.PhieuCanCaGiongVungNuoiDaiThanhSide();
            return dao.GetTongHops<T>(fromDate,toDate);
        }
        #endregion
    }
}
