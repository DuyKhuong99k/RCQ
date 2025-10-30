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
using Models.Repos;
using System.Globalization;

namespace ViewModels.Repos.HQ
{
    public partial class ThanhPhamFilletViewModel : ObservableObject
    {
        private static ThanhPhamFilletViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private MaThanhPhamFillet? item;
        [ObservableProperty] private ObservableRangeCollection<MaThanhPhamFillet> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private MaThanhPhamFillet? selectedItem;
        [ObservableProperty] private string selectedValue;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ObservableRangeCollection<MaThanhPhamFillet> usedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private ThanhPhamFilletViewModel()
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

        public static ThanhPhamFilletViewModel Instance => instance ??= new ThanhPhamFilletViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public MaThanhPhamFillet CopyItem(MaThanhPhamFillet item)
        {
            return new MaThanhPhamFillet
            {
                SuDung = item.SuDung,
                Ma = item?.Ma,
                Ten = item?.Ten,
                Min = item?.Min,
                Max = item?.Max,
                IsSoChe = item?.IsSoChe ?? false,
                IsCaMuoi = item?.IsCaMuoi ?? false,
                DinhMuc = item?.DinhMuc ?? 0,
                BravoId = item?.BravoId,
                ThoiGianTren1kgSeconds = item?.ThoiGianTren1kgSeconds ?? 0,
                DinhMucHaoHut = item?.DinhMucHaoHut ?? 0,
                CodeId = item?.CodeId,
                IsNotSetByTime = item?.IsNotSetByTime ?? false,
                ColorRGB = item?.ColorRGB,
                KhongPhanBietSize = item?.KhongPhanBietSize ?? false,
                IsXeBuom = item?.IsXeBuom ?? false,
                IsChuyenFillet = item?.IsChuyenFillet ?? false,
                IsDat = item?.IsDat ?? false,
                IsNguyenLieuXeBuom = item?.IsNguyenLieuXeBuom ?? false,
                IsNguyenCon = item?.IsNguyenCon ?? false,
                
            };
        }
        public MaThanhPhamFillet CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public MaThanhPhamFillet CreateDefaultNew()
        {
            var maxId = Items.Where(x => int.TryParse(x.Ma, out var rl))
                   .Select(x => int.Parse(x.Ma))
                   .DefaultIfEmpty(0)
                   .Max();
            var id = $"{(maxId + 1).ToString()}";
            return new MaThanhPhamFillet
            {
                SuDung = true,
                Ma = id
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.MaThanhPhamFillet();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(MaThanhPhamFillet item)
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
            var dao = new Dao.Repos.HQ.MaThanhPhamFillet();
            return dao.Gets<T>();
        }
        public List<MaThanhPhamFillet_U> GetUs(string Ngay, int PageIndex, int PageSize)
        {
            var db = new dbPMScontext();
            var date = new DateTime();
            date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            var latestDates = db.MaThanhPhamFilletUs
                .Where(x => x.MNgay.Date >= date.Date)
                .GroupBy(x => x.MaThanhPham)
                .Select(g => new { MaThanhPham = g.Key, MaxId = g.Max(x => x.Id) });

            var query = from hq in db.MaThanhPhamFilletUs.Where(x => x.MNgay.Date >= date.Date) 
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
        public List<MaThanhPhamFillet_D> GetDs(string Ngay, int PageIndex, int PageSize)
        {
            var db = new dbPMScontext();
            var date = new DateTime();
            date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            var latestDates = db.MaThanhPhamFilletDs
                .Where(x => x.MNgay.Date >= date.Date)
                .GroupBy(x => x.MaThanhPham)
                .Select(g => new { MaThanhPham = g.Key, MaxId = g.Max(x => x.Id) });

            var query = from hq in db.MaThanhPhamFilletDs.Where(x => x.MNgay.Date >= date.Date) 
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
            var dao = new Dao.Repos.HQ.MaThanhPhamFillet();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(MaThanhPhamFillet item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<MaThanhPhamFillet>();
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
        private bool IsItemPass(MaThanhPhamFillet item)
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

            var items = Gets<MaThanhPhamFillet>();
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
        private void Reload_(ObservableRangeCollection<MaThanhPhamFillet> obj)
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
            var dao = new Dao.Repos.HQ.MaThanhPhamFillet();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(MaThanhPhamFillet item)
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
        public MaThanhPhamFillet? Find(string ma)
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

        public bool Exists(MaThanhPhamFillet item)
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
