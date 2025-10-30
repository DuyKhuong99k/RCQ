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
    public partial class PhieuCanNguyenLieuViewModel : ObservableObject
    {
        private static PhieuCanNguyenLieuViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private PhieuCanNguyenLieu? item;
        [ObservableProperty] private ObservableRangeCollection<PhieuCanNguyenLieu> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private PhieuCanNguyenLieu? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private PhieuCanNguyenLieuViewModel()
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

        public static PhieuCanNguyenLieuViewModel Instance => instance ??= new PhieuCanNguyenLieuViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public List<T> GetTongHopThanhPhamDashBoard<T>(DateTime fromDate, DateTime toDate, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu();
            return dao.GetTongHopThanhPhamDashBoard<T>(fromDate.Date, toDate.Date, xuongId);
        }

        //public PhieuCanNguyenLieu CopyItem(PhieuCanNguyenLieu item)
        //{
        //    return new PhieuCanNguyenLieu
        //    {
        //        SuDung = item.SuDung,
        //        Ma = item?.Ma,
        //        Ten = item?.Ten
        //    };
        //}
        //public PhieuCanNguyenLieu CopySelectedItem()
        //{
        //    return CopyItem(SelectedItem);
        //}

        //public PhieuCanNguyenLieu CreateDefaultNew()
        //{
        //    var maxId = Items.Where(x => int.TryParse(x.Ma, out var rl))
        //           .Select(x => int.Parse(x.Ma))
        //           .DefaultIfEmpty(0)
        //           .Max();
        //    var id = $"{(maxId + 1).ToString()}";
        //    return new PhieuCanNguyenLieu
        //    {
        //        SuDung = true,
        //        Ma = id
        //    };
        //}

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu();
            return dao.Delete(item);
        }

        //[RelayCommand(CanExecute = nameof(IsItemPass))]
        //private void Delete_(PhieuCanNguyenLieu item)
        //{
        //    try
        //    {
        //        if (Delete(item) > 0)
        //            lock (Items)
        //            {
        //                var _item = Items.SingleOrDefault(x => x.Ma == item.Ma);
        //                if (_item != null)
        //                {
        //                    var index = Items.IndexOf(_item);
        //                    Items.RemoveAt(index);
        //                    //Items.Insert(index,item);
        //                }
        //            }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        //throw;
        //        VmMessage.SetExceptionCommand.Execute(e);
        //    }
        //}

        private List<T> Gets<T>()
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu();
            return dao.Gets<T>();
        }
        public List<T> Gets<T>(DateTime dateTime)
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu();
            return dao.Gets<T>(dateTime);
        }

        public int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu();
            return dao.Insert(item);
        }

        //[RelayCommand(CanExecute = nameof(IsItemPass))]
        //private void Insert_(PhieuCanNguyenLieu item)
        //{
        //    try
        //    {
        //        if (Insert(item) > 0)
        //        {
        //            var items = new List<PhieuCanNguyenLieu>();
        //            lock (Items)
        //            {
        //                Items.Add(item);
        //            }
        //            VmMessage.MessageBoxShow("Thực Hiện Xong", "Thông Báo", 0);
        //            IsWindowItemShown = false;
        //        }
        //        else
        //        {
        //            VmMessage.MessageBoxShow("Không thể thêm", "Thông Báo", 0);
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        //throw;
        //        VmMessage.SetExceptionCommand.Execute(e);
        //    }
        //}

        //[RelayCommand()]
        //private void Insert2_()
        //{
        //    try
        //    {
        //        if (Insert(Item) > 0)
        //        {
        //            Items.Insert(0, Item);
        //            VmMessage.MessageBoxShow("Thực Hiện Xong", "Thông Báo", 0);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        VmMessage.SetExceptionCommand.Execute(e);
        //        //throw;
        //    }
        //}
        public bool IsVailSelectedItem => SelectedItem != null;
        //private bool IsItemPass(PhieuCanNguyenLieu item)
        //{
        //    return item != null && item.Ten != null && item.Ten.Trim() != "" &&
        //           item.SuDung != null;
        //}
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

            var items = Gets<PhieuCanNguyenLieu>();
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
        private void Reload_(ObservableRangeCollection<PhieuCanNguyenLieu> obj)
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
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu();
            return dao.Update(item);
        }

        public List<T> GetsLast<T>(DateTime dateTime, int num)
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu();
            return dao.GetsLast<T>(dateTime, num);
        }
        //[RelayCommand(CanExecute = nameof(IsItemPass))]
        //private void Update_(PhieuCanNguyenLieu item)
        //{
        //    try
        //    {
        //        if (Update(item) > 0)
        //            lock (Items)
        //            {
        //                var _item = Items.SingleOrDefault(x => x.Ma == item.Ma);
        //                if (_item != null)
        //                {
        //                    var index = Items.IndexOf(_item);
        //                    Items.RemoveAt(index);
        //                    Items.Insert(index, item);
        //                }
        //            }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        //throw;
        //        VmMessage.SetExceptionCommand.Execute(e);
        //    }
        //}
        //[RelayCommand()]
        //private void Update2_()
        //{
        //    try
        //    {
        //        if (Update(Item) > 0)
        //            lock (Items)
        //            {
        //                var _item = Items.SingleOrDefault(x => x.Ma == Item.Ma);
        //                if (_item != null)
        //                {
        //                    var index = Items.IndexOf(_item);
        //                    Items.RemoveAt(index);
        //                    Items.Insert(index, Item);
        //                    SelectedItem = Item;
        //                }
        //            }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        //throw;
        //        VmMessage.SetExceptionCommand.Execute(e);
        //    }
        //}
        #region Xử Lý Phiếu Cân
        public List<T> GetPhieuCan<T>(DateTime dateTime, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
            return dao.GetPhieuCan<T>(dateTime, xuongId);
        }
        public List<Tuple<string, DateTime>> GetAos(string xuongId, int topVal = 4, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
                return dao.GetsAo(xuongId, topVal);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region Tính Lương Phụ Fillet
        public double GetSanLuongNguyenLieu(TimeSpan fromTime, TimeSpan toTime, DateTime dateTime, string xuongId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
                return dao.GetSanLuong(fromTime, toTime, dateTime, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public double GetSanLuongNguyenLieu(DateTime dateTime, string xuongId, string banId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
                return dao.GetSanLuong(dateTime, xuongId, banId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public double GetSanLuongNguyenLieuTruNgop(
            TimeSpan fromTime,
            TimeSpan toTime,
            DateTime dateTime,
            string xuongId,
            string banId,
            string sizeId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
                return dao.GetSanLuongTruNgop(fromTime, toTime, dateTime, xuongId, banId, sizeId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public double GetSanLuongCaNgopTruBan(
            DateTime dateTime,
            string xuongId,
            string sizeId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
                return dao.GetSanLuongCaNgopTruBan(dateTime, xuongId, sizeId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<string> GetMSLs(
            TimeSpan fromTime,
            TimeSpan toTime,
            DateTime dateTime,
            string xuongId,
            string sizeId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
                return dao.GetMSLs(fromTime, toTime, dateTime, xuongId, sizeId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public double GetSanLuongCaNgop(
            TimeSpan fromTime,
            TimeSpan toTime,
            DateTime dateTime,
            string xuongId,
            string banId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
                return dao.GetSanLuongCaNgop(fromTime, toTime, dateTime, xuongId, banId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region Báo Cáo
        public List<T> GetPhieuCanChiTiets<T>(DateTime fromDate, DateTime toDate, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
            return dao.GetChiTiets<T>(fromDate, toDate);
        }
        public List<T> GetPhieuCanTongHopNCCs<T>(DateTime fromDate, DateTime toDate, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
            return dao.GetTongHopNCC<T>(fromDate, toDate);
        }
        //BÀN CẮT TIẾT
        public List<T> GetPhieuCanTongHopBCTs<T>(DateTime fromDate, DateTime toDate, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
            return dao.GetTongHopBCT<T>(fromDate, toDate);
        }
        public List<T> GetPhieuCanTongHopPhuongTiens<T>(DateTime fromDate, DateTime toDate, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
            return dao.GetTongHopPhuongTien<T>(fromDate, toDate);
        }
        #endregion
        public List<T> GetsCaTra<T>(DateTime dateTime, string xuongId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu(connStr);
            return dao.GetsCaTra<T>(dateTime, xuongId);
        }
    }
}
