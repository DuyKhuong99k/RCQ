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
using System.Globalization;
using Models.Repos;

namespace ViewModels.Repos.HQ
{
    public partial class ThanhPhamNguyenLieuViewModel : ObservableObject
    {
        private static ThanhPhamNguyenLieuViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private MaThanhPhamNguyenLieu? item;
        [ObservableProperty] private ObservableRangeCollection<MaThanhPhamNguyenLieu> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private MaThanhPhamNguyenLieu? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ObservableRangeCollection<MaThanhPhamNguyenLieu> usedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private ThanhPhamNguyenLieuViewModel()
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

        public static ThanhPhamNguyenLieuViewModel Instance => instance ??= new ThanhPhamNguyenLieuViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public MaThanhPhamNguyenLieu CopyItem(MaThanhPhamNguyenLieu item)
        {
            return new MaThanhPhamNguyenLieu
            {

                Ma = item.Ma,
                MaCa = item.MaCa,
                Max = item.Max,
                Min = item.Min,
                SuDung = item.SuDung,
                Ten = item.Ten,
                TyLeNuoc = item.TyLeNuoc,
                IsSNL = item.IsSNL,
                IsPhuPhamCaTap = item.IsPhuPhamCaTap,
                IsNgopXePhuPham = item.IsNgopXePhuPham,
                IsNgopGhePhuPham = item.IsNgopGhePhuPham,
                IsNgopGheMuoi = item.IsNgopGheMuoi,
                IsNgopGhe = item.IsNgopGhe,
                IsCaCanTin = item.IsCaCanTin,
                IsCaNgopXeMuoi = item.IsCaNgopXeMuoi,
                IsDatNho = item.IsDatNho,
                IsManh = item.IsManh,
                IsMuoiGhePhuPham = item.IsMuoiGhePhuPham,
                IsNgopAoMuoi = item.IsNgopAoMuoi,
                IsNgopAoPhuPham = item.IsNgopAoPhuPham,
                IsCaNgopGheAoBanNgoai = item.IsCaNgopGheAoBanNgoai,
                IsCaNgopGheTuoiBanNgoai = item.IsCaNgopGheTuoiBanNgoai,
                IsTareThung = item.IsTareThung,
                MNgay = item.MNgay,
                CTTYLE = item.CTTYLE,
            };
        }
        public MaThanhPhamNguyenLieu CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public MaThanhPhamNguyenLieu CreateDefaultNew()
        {
            var maxId = Items.Where(x => int.TryParse(x.Ma, out var rl)).Select(x => x.Ma).DefaultIfEmpty("0")
                .Max();
            var id = (int.Parse(maxId) + 1).ToString("000");
            return new MaThanhPhamNguyenLieu
            {
                Ma = id,
                SuDung = true,
                Min = 0,
                Max = 999,
                MaCa = "A", //LoaiCaNguyenLieuViewModel.Ins.Items.Where(x => x.SuDung == true).FirstOrDefault()?.Ma
                IsCaCanTin = false,
                IsCaNgopXeMuoi = false,
                IsDatNho = false,
                IsManh = false,
                IsMuoiGhePhuPham = false,
                IsNgopAoMuoi = false,
                IsNgopAoPhuPham = false,
                IsNgopGhe = false,
                IsNgopGheMuoi = false,
                IsNgopGhePhuPham = false,
                IsNgopXePhuPham = false,
                IsPhuPhamCaTap = false,
                IsSNL = false,
                TyLeNuoc = 0,
                IsTareThung = false,
                IsCaNgopGheTuoiBanNgoai = false,
                IsCaNgopGheAoBanNgoai = false,
                MNgay = DateTime.Now,
                CTTYLE = "SL/NLFILLET"
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.MaThanhPhamNguyenLieu();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(MaThanhPhamNguyenLieu item)
        {
            try
            {
                if (Delete(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.Ma == item.Ma);
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
            var dao = new Dao.Repos.HQ.MaThanhPhamNguyenLieu();
            return dao.Gets<T>();
        }
        public List<MaThanhPhamNguyenLieu_U> GetUs(string Ngay, int PageIndex, int PageSize)
        {
            var db = new dbPMScontext();
            var date = new DateTime();
            date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            var latestDates = db.MaThanhPhamNguyenLieuUs
                .Where(x => x.MNgay.Date >= date.Date)
                .GroupBy(x => x.MaThanhPham)
                .Select(g => new { MaThanhPham = g.Key, MaxId = g.Max(x => x.Id) });

            var query = from hq in db.MaThanhPhamNguyenLieuUs.Where(x => x.MNgay.Date >= date.Date) 
                join latest in latestDates
                    on new { hq.MaThanhPham, hq.Id } 
                    equals new { latest.MaThanhPham, Id = latest.MaxId }
                orderby hq.MNgay descending
                select hq;

            var result = query.Skip((PageIndex - 1) * PageSize)
                .Take(PageSize)
                .ToList();
            return result;
        }
        public List<MaThanhPhamNguyenLieu_D> GetDs(string Ngay, int PageIndex, int PageSize)
        {
            var db = new dbPMScontext();
            var date = new DateTime();
            date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            var latestDates = db.MaThanhPhamNguyenLieuDs
                .Where(x => x.MNgay.Date >= date.Date)
                .GroupBy(x => x.MaThanhPham)
                .Select(g => new { MaThanhPham = g.Key, MaxId = g.Max(x => x.Id) });

            var query = from hq in db.MaThanhPhamNguyenLieuDs.Where(x => x.MNgay.Date >= date.Date) 
                join latest in latestDates
                    on new { hq.MaThanhPham, hq.Id } 
                    equals new { latest.MaThanhPham, Id = latest.MaxId }
                orderby hq.MNgay descending
                select hq;

            var result = query.Skip((PageIndex - 1) * PageSize)
                .Take(PageSize)
                .ToList();
            return result;
        }
        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.MaThanhPhamNguyenLieu();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(MaThanhPhamNguyenLieu item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<MaThanhPhamNguyenLieu>();
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
        private bool IsItemPass(MaThanhPhamNguyenLieu item)
        {
            return item != null && item.Ten != null && item.Ten.Trim() != "" &&
                   item.SuDung != null;
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
                UsedItems.Clear();
            }

            var items = Gets<MaThanhPhamNguyenLieu>();
            if (items.Any())
                lock (Items)
                {
                    try
                    {
                        //Items.AddRange(items);
                        foreach (var item in items)
                        {
                            Items.Add(item);
                            if (item.SuDung == true)
                            {
                                UsedItems.Add(item);
                            }
                        }

                    }
                    catch (NotSupportedException e)
                    {

                    }

                }


        }

        [RelayCommand]
        private void Reload_(ObservableRangeCollection<MaThanhPhamNguyenLieu> obj)
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
            var dao = new Dao.Repos.HQ.MaThanhPhamNguyenLieu();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(MaThanhPhamNguyenLieu item)
        {
            try
            {
                if (Update(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.Ma == item.Ma);
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
                        var _item = Items.SingleOrDefault(x => x.Ma == Item.Ma);
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
        public MaThanhPhamNguyenLieu? Find(string ma)
        {
            try
            {
                return Items.FirstOrDefault(x => x.Ma == ma);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Exists(MaThanhPhamNguyenLieu item)
        {
            try
            {

                return Find(item.Ma) != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
