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
    public partial class AoViewModel : ObservableObject
    {
        private static AoViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private MaAoVungNuoi? item;
        [ObservableProperty] private ObservableRangeCollection<MaAoVungNuoi> items = new();
        [ObservableProperty] private ObservableRangeCollection<Ao> itemsAo = new(); 
        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private MaAoVungNuoi? selectedItem;
        [ObservableProperty] private Ao? selectedItemAo;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ObservableRangeCollection<object> selectedItemsAo = new();
        [ObservableProperty] private ObservableRangeCollection<Ao> usedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private AoViewModel()
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

        public static AoViewModel Instance => instance ??= new AoViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public MaAoVungNuoi CopyItem(MaAoVungNuoi item)
        {
            return new MaAoVungNuoi
            {
                SuDung = item.SuDung,
                Ma = item?.Ma,
                Ten = item?.Ten
            };
        }
        public MaAoVungNuoi CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public MaAoVungNuoi CreateDefaultNew()
        {
            var maxId = Items.Where(x => int.TryParse(x.Ma, out var rl))
                   .Select(x => int.Parse(x.Ma))
                   .DefaultIfEmpty(0)
                   .Max();
            var id = $"{(maxId + 1).ToString()}";
            return new MaAoVungNuoi
            {
                SuDung = true,
                Ma = id
            };
        }
        public Ao CopyItemAo(Ao item)
        {
            return new Ao
            {
                SuDung = item.SuDung,
                Ma = item?.Ma,
                Ten = item?.Ten,
                MNgay = item.MNgay
            };
        }
        public Ao CopySelectedItemAo()
        {
            return CopyItemAo(SelectedItemAo);
        }
        public List<Ao_U> GetUs(string Ngay,int PageIndex,int PageSize)
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
            ////return db.AoUs.Where(x=>x.MNgay.Date >= date.Date).OrderByDescending(x=>x.MNgay).Skip((PageIndex -1)*PageSize).Take(PageSize).ToList();
            //return db.AoUs .Where(x => x.MNgay.Date >= date.Date)
            //    .GroupBy(x => x.MaAo)
            //    .Select(g => g.OrderByDescending(x => x.MNgay).FirstOrDefault())
            //    .Where(x => x != null) 
            //    .OrderByDescending(x => x!.MNgay)
            //    .Skip((PageIndex - 1) * PageSize)
            //    .Take(PageSize)
            //    .ToList();
            var latestDates = db.AoUs
                .Where(x => x.MNgay.Date >= date.Date)
                .GroupBy(x => x.MaAo)
                .Select(g => new { MaAo = g.Key, MaxId = g.Max(x => x.Id) });

            var query = from hq in db.AoUs.Where(x => x.MNgay.Date >= date.Date) 
                join latest in latestDates
                    on new { hq.MaAo, hq.Id } 
                    equals new { latest.MaAo, Id = latest.MaxId }
                orderby hq.MNgay descending
                select hq;

            var result = query.Skip((PageIndex - 1) * PageSize)
                .Take(PageSize)
                .ToList();
            return result;
        }
        public List<Ao_D> GetDs(string Ngay,int PageIndex,int PageSize)
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
            return db.AoDs.Where(x=>x.MNgay.Date >= date.Date).OrderByDescending(x=>x.MNgay).Skip((PageIndex -1)*PageSize).Take(PageSize).ToList();
        }
        public Ao CreateDefaultNewAo()
        {
            var maxId = Items.Where(x => int.TryParse(x.Ma, out var rl))
                .Select(x => int.Parse(x.Ma))
                .DefaultIfEmpty(0)
                .Max();
            var id = $"{(maxId + 1).ToString()}";
            return new Ao
            {
                SuDung = true,
                Ma = id,
                MNgay = DateTime.Now
            };
        }
        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.MaAoVungNuoi();
            return dao.Delete(item);
        }
        public int DeleteAo<T>(T item)
        {
            var dao = new Dao.Repos.HQ.Ao();
            return dao.Delete(item);
        }
        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(MaAoVungNuoi item)
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
            var dao = new Dao.Repos.HQ.MaAoVungNuoi();
            return dao.Gets<T>();
        }
        public List<T> GetsAo<T>()
        {
            var dao = new Dao.Repos.HQ.Ao();
            return dao.Gets<T>();
        }
        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.MaAoVungNuoi();
            return dao.Insert(item);
        }
        public int InsertAo<T>(T item)
        {
            var dao = new Dao.Repos.HQ.Ao();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(MaAoVungNuoi item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<MaAoVungNuoi>();
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
        private bool IsItemPass(MaAoVungNuoi item)
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
            }

            var items = Gets<MaAoVungNuoi>();
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
            lock (ItemsAo)
            {
                ItemsAo.Clear();
                UsedItems.Clear();
            }

            var itemsAo = GetsAo<Ao>();
            if (itemsAo.Any())
                lock (ItemsAo)
                {
                    try
                    {
                        //Items.AddRange(items);
                        foreach (var item in itemsAo)
                        {
                            ItemsAo.Add(item);
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
        private void Reload_(ObservableRangeCollection<MaAoVungNuoi> obj)
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
            var dao = new Dao.Repos.HQ.MaAoVungNuoi();
            return dao.Update(item);
        }
        public int UpdateAo<T>(T item)
        {
            var dao = new Dao.Repos.HQ.Ao();
            return dao.Update(item);
        }
        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(MaAoVungNuoi item)
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
        public MaAoVungNuoi? Find(string ma)
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
        public Ao? FindAo(string ma)
        {
            try
            {
                return ItemsAo.FirstOrDefault(x => x.Ma == ma);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Exists(string ma)
        {
            try
            {

                return Find(ma) != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public bool ExistsAo(string ma)
        {
            try
            {

                return FindAo(ma) != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
    }
}
