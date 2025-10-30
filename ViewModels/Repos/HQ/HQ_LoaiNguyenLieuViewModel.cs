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
    public partial class HQ_LoaiNguyenLieuViewModel : ObservableObject
    {
        private static HQ_LoaiNguyenLieuViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private HQ_LoaiNguyenLieu? item;
        [ObservableProperty] private ObservableRangeCollection<HQ_LoaiNguyenLieu> items = new();
        [ObservableProperty] private ObservableRangeCollection<HQ_LoaiNguyenLieu> usedItems = new();
        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private HQ_LoaiNguyenLieu? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private HQ_LoaiNguyenLieuViewModel()
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

        public static HQ_LoaiNguyenLieuViewModel Instance => instance ??= new HQ_LoaiNguyenLieuViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public HQ_LoaiNguyenLieu CopyItem(HQ_LoaiNguyenLieu item)
        {
            return new HQ_LoaiNguyenLieu
            {
                SuDung = item.SuDung,
                Id = item?.Id,
                Ten = item?.Ten,
                //MNgay = item?.MNgay
            };
        }
        public HQ_LoaiNguyenLieu CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public HQ_LoaiNguyenLieu CreateDefaultNew()
        {
            var maxId = Items.Where(x => int.TryParse(x.Id, out var rl))
                   .Select(x => int.Parse(x.Id))
                   .DefaultIfEmpty(0)
                   .Max();
            var id = $"{(maxId + 1).ToString()}";
            return new HQ_LoaiNguyenLieu
            {
                SuDung = true,
                Id = id
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.HQ_LoaiNguyenLieu();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(HQ_LoaiNguyenLieu item)
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
            var dao = new Dao.Repos.HQ.HQ_LoaiNguyenLieu();
            return dao.Gets<T>();
        }
        public List<HQ_LoaiNguyenLieu_D> GetDs(string Ngay,int PageIndex,int PageSize)
        {
            dbPMScontext db = new dbPMScontext();
            DateTime date = new DateTime();
            try
            {
                date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss",CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return db.HqLoaiNguyenLieuDs.Where(x=>x.MNgay.Date >= date.Date).OrderByDescending(x=>x.MNgay).Skip((PageIndex -1)*PageSize).Take(PageSize).ToList();
        }

        public List<HQ_LoaiNguyenLieu_U> GetUs(string Ngay,int PageIndex,int PageSize)
        {
            dbPMScontext db = new dbPMScontext();
            DateTime date = new DateTime();
            try
            {
                date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss",CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            var latestDates = db.HqLoaiNguyenLieuUs
                .Where(x => x.MNgay.Date >= date.Date)
                .GroupBy(x => x.LoaiNguyenLieuId)
                .Select(g => new { LoaiNguyenLieuId = g.Key, MaxId = g.Max(x => x.Id) });

            var query = from hq in db.HqLoaiNguyenLieuUs.Where(x => x.MNgay.Date >= date.Date) 
                join latest in latestDates
                    on new { hq.LoaiNguyenLieuId, hq.Id } 
                    equals new { latest.LoaiNguyenLieuId, Id = latest.MaxId }
                orderby hq.MNgay descending
                select hq;

            var result = query.Skip((PageIndex - 1) * PageSize)
                .Take(PageSize)
                .ToList();
            return result;
        }
        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.HQ_LoaiNguyenLieu();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(HQ_LoaiNguyenLieu item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<HQ_LoaiNguyenLieu>();
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

        private bool IsItemPass(HQ_LoaiNguyenLieu item)
        {
            return item != null && item.Ten != null && item.Ten.Trim() != "" &&
                   item.SuDung != null;
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
                UsedItems.Clear();
            }

            var items = Gets<HQ_LoaiNguyenLieu>();
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
        private void Reload_(ObservableRangeCollection<HQ_LoaiNguyenLieu> obj)
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
            var dao = new Dao.Repos.HQ.HQ_LoaiNguyenLieu();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(HQ_LoaiNguyenLieu item)
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
        public HQ_LoaiNguyenLieu? Find(string ma)
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

        public bool Exists(HQ_LoaiNguyenLieu item)
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
    }
}
