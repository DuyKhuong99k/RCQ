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
    public partial class HQ_LoViewModel : ObservableObject
    {
        private static HQ_LoViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private string? item;
        [ObservableProperty] private ObservableRangeCollection<string> items = new();
        [ObservableProperty] private string selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();

        [ObservableProperty] private ObservableRangeCollection<Tuple<DateTime?, string, string>> _itemsWithSize = new ObservableRangeCollection<Tuple<DateTime?, string, string>>();
        [ObservableProperty] private Tuple<DateTime?, string, string> _selectedItemWithSize;
        [ObservableProperty] private ObservableRangeCollection<string> defaultLos = new ObservableRangeCollection<string>();
        //[ObservableProperty] private ObservableRangeCollection<string> _items = new ObservableRangeCollection<string>();
        [ObservableProperty] private bool isServer = true;



        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;

        private readonly SynchronizationContext synchronizationContext;

        public List<string> GetDefaultLos()
        {
            try
            {
                var items = new List<string>();
                for (int i = 1; i <= 50; i++)
                {
                    var item = $@"{AppViewModel.Instance.DateReport:yyyyMMdd}.{i:00}";
                    items.Add(item);
                }

                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private HQ_LoViewModel()
        {
            try
            {
                // Reload();
                Items.Clear();
                var items = new ObservableRangeCollection<string>(Gets(AppViewModel.Instance.XuongId, IsServer));
                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        Items.Add(item);

                    }
                }
                ItemsWithSize = new ObservableRangeCollection<Tuple<DateTime?, string, string>>(
                    GetsWithSize(AppViewModel.Instance.XuongId, IsServer));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                VmMessage.SetExceptionCommand.Execute(e);
            }
        }
        public List<string> Gets(string xuongId, bool isServer = false)
        {
            try
            {

                var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu();
                if (AppViewModel.Instance.LoKv == Vars.AppKV.TPDinhHinh)
                    return dao.GetsMSLDinhHinh(xuongId);
                else if (AppViewModel.Instance.LoKv == Vars.AppKV.TPFillet)
                {
                    return dao.GetsMSLTPFillet(xuongId);
                }
                else if (AppViewModel.Instance.LoKv == Vars.AppKV.BTPDinhHinh)
                {
                    return dao.GetsMSLDinhHinh_BTP(xuongId);
                }
                else if (AppViewModel.Instance.LoKv == Vars.AppKV.BTPFilletv2)
                {
                    return dao.GetsMSLBTPFilletv2(xuongId);
                }
                else
                {
                    return dao.GetsMSL(xuongId);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Tuple<DateTime?, string, string>> GetsLastWithSize(string xuongId, bool isServer = false)
        {
            try
            {

                var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu();
                if (AppViewModel.Instance.LoKv == Vars.AppKV.TPDinhHinh)
                {
                    if (AppViewModel.Instance.LoKv == Vars.AppKV.TPDinhHinh)
                        return dao.GetsLastMSLWithSizeDinhHinh(xuongId);
                }
                else if (AppViewModel.Instance.LoKv == Vars.AppKV.CaoThit)
                {
                    return dao.GetsLastMSLWithSizeCaoThit(xuongId);
                }
                else if (AppViewModel.Instance.LoKv == Vars.AppKV.TPFillet)
                {
                    return dao.GetsLastMSLWithSizeTPFillet(xuongId);
                }
                else if (AppViewModel.Instance.LoKv == Vars.AppKV.BTPFilletv2)
                {
                    return dao.GetsLastMSLWithSizeBTPFilletv2(xuongId);
                }
                else if (AppViewModel.Instance.LoKv == Vars.AppKV.BTPDinhHinh)
                {
                    return dao.GetsLastMSLWithSizeDinhHinh_BTP(xuongId);
                }

                return dao.GetsLastMSLWithSize(xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Tuple<DateTime?, string, string>> GetsWithSize(string xuongId, bool isServer = false)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuCanNguyenLieu();
                if (AppViewModel.Instance.LoKv == Vars.AppKV.TPDinhHinh)
                {
                    if (AppViewModel.Instance.LoKv == Vars.AppKV.TPDinhHinh)
                        return dao.GetsMSLWithSizeDinhHinh(xuongId);
                }
                else if (AppViewModel.Instance.LoKv == Vars.AppKV.CaoThit)
                {
                    return dao.GetsMSLWithSizeCaoThit(xuongId);
                }
                else if (AppViewModel.Instance.LoKv == Vars.AppKV.BTPDinhHinh)
                {
                    return dao.GetsMSLWithSizeDinhHinh_BTP(xuongId);
                }
                else if (AppViewModel.Instance.LoKv == Vars.AppKV.TPFillet)
                {
                    return dao.GetsMSLWithSizeTPFillet(xuongId);
                }
                else if (AppViewModel.Instance.LoKv == Vars.AppKV.BTPFilletv2)
                {
                    return dao.GetsMSLWithSizeBTPFilletv2(xuongId);
                }

                return dao.GetsMSLWithSize(xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static HQ_LoViewModel Instance => instance ??= new HQ_LoViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public MaLo CopyItem(MaLo item)
        {
            return new MaLo
            {
                SuDung = item.SuDung,
                Ma = item?.Ma,
                Ten = item?.Ten
            };
        }
        public List<HQ_Lo_D> GetDs(string Ngay, int PageIndex, int PageSize)
        {
            dbPMScontext db = new dbPMScontext();
            DateTime date = new DateTime();
            try
            {
                date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return db.HqLoDs.Where(x => x.MNgay > date).OrderByDescending(x => x.MNgay).Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
        }

        public List<HQ_Lo_U> GetUs(string Ngay, int PageIndex, int PageSize)
        {
            dbPMScontext db = new dbPMScontext();
            DateTime date = new DateTime();
            try
            {
                date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return db.HqLoUs.Where(x => x.MNgay > date).OrderByDescending(x => x.MNgay).Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
        }
        public List<HQ_Lo> Gets(string Ngay, int PageIndex, int PageSize)
        {
            dbPMScontext db = new dbPMScontext();
            DateTime date = new DateTime();
            try
            {
                date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return db.HqLos.Where(x => x.MNgay > date).OrderByDescending(x => x.MNgay).Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
        }
        //public MaLo CopySelectedItem()
        //{
        //    return CopyItem(SelectedItem);
        //}

        //public MaLo CreateDefaultNew()
        //{
        //    var maxId = Items.Where(x => int.TryParse(x.Ma, out var rl))
        //           .Select(x => int.Parse(x.Ma))
        //           .DefaultIfEmpty(0)
        //           .Max();
        //    var id = $"{(maxId + 1).ToString()}";
        //    return new MaLo
        //    {
        //        SuDung = true,
        //        Ma = id
        //    };
        //}

        //private int Delete<T>(T item)
        //{
        //    var dao = new Dao.Repos.HQ.MaLo();
        //    return dao.Delete(item);
        //}

        //[RelayCommand(CanExecute = nameof(IsItemPass))]
        //private void Delete_(MaLo item)
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

        //private List<T> Gets<T>()
        //{
        //    var dao = new Dao.Repos.HQ.MaLo();
        //    return dao.Gets<T>();
        //}

        //private int Insert<T>(T item)
        //{
        //    var dao = new Dao.Repos.HQ.MaLo();
        //    return dao.Insert(item);
        //}

        //[RelayCommand(CanExecute = nameof(IsItemPass))]
        //private void Insert_(MaLo item)
        //{
        //    try
        //    {
        //        if (Insert(item) > 0)
        //        {
        //            var items = new List<MaLo>();
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
        //public bool IsVailSelectedItem => SelectedItem != null;
        //private bool IsItemPass(MaLo item)
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
        //public void Reload()
        //{
        //    lock (Items)
        //    {
        //        Items.Clear();
        //    }

        //    var items = Gets<MaLo>();
        //    if (items.Any())
        //        lock (Items)
        //        {
        //            try
        //            {
        //                //Items.AddRange(items);
        //                foreach (var item in items)
        //                {
        //                    Items.Add(item);
        //                }
        //            }
        //            catch (NotSupportedException e)
        //            {

        //            }

        //        }


        //}

        //[RelayCommand]
        //private void Reload_(ObservableRangeCollection<MaLo> obj)
        //{
        //    try
        //    {
        //        Reload();
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        //throw;
        //        VmMessage.SetExceptionCommand.Execute(e);
        //    }
        //}

        //private int Update<T>(T item)
        //{
        //    var dao = new Dao.Repos.HQ.MaLo();
        //    return dao.Update(item);
        //}

        //[RelayCommand(CanExecute = nameof(IsItemPass))]
        //private void Update_(MaLo item)
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
        public string? Find(string ma)
        {
            try
            {
                return Items.FirstOrDefault(x => x == ma);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Exists(string item)
        {
            try
            {

                return Find(item) != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
