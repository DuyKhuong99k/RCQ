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
    public partial class HQ_NhanVienTheoNhomViewModel : ObservableObject
    {
        private static HQ_NhanVienTheoNhomViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private HQ_NhanVienTheoNhom? item;
        [ObservableProperty] private ObservableRangeCollection<HQ_NhanVienTheoNhom> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private HQ_NhanVienTheoNhom? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;

        private HQ_NhanVienTheoNhomViewModel()
        {
            try
            {
                //Reload();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                //VmMessage.SetExceptionCommand.Execute(e);
            }
        }

        public static HQ_NhanVienTheoNhomViewModel Instance => instance ??= new HQ_NhanVienTheoNhomViewModel();
        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;
        public HQ_NhanVienTheoNhom CopyItem(HQ_NhanVienTheoNhom item)
        {
            return new HQ_NhanVienTheoNhom
            {
                Id = item.Id,
                MaNhanVien = item.MaNhanVien,
                MaNhom = item.MaNhom,
                NgayGioBatDau = item.NgayGioBatDau,
                HeSo = item.HeSo
            };
        }
        public HQ_NhanVienTheoNhom CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public HQ_NhanVienTheoNhom CreateDefaultNew()
        {
            var maxId = Items
                   .Select(x => x.Id)
                   .DefaultIfEmpty(0)
                   .Max();
            var id = $"{(maxId + 1).ToString()}";
            return new HQ_NhanVienTheoNhom
            {
                Id = maxId
            };
        }

        public int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.HQ_NhanVienTheoNhom();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(HQ_NhanVienTheoNhom item)
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
            var dao = new Dao.Repos.HQ.HQ_NhanVienTheoNhom();
            return dao.Gets<T>();
        }
        public List<T> Gets<T>(DateTime dateTime)
        {
            var dao = new Dao.Repos.HQ.HQ_NhanVienTheoNhom();
            return dao.GetNews<T>(dateTime);
        }
        public HQ_NhanVienTheoNhom GetByMa(long id)
        {
            dbPMScontext db = new dbPMScontext();
            var item = db.HqNhanVienTheoNhoms.FirstOrDefault(x => x.Id == id);
            return item;
        }
        //public List<HQ_NhanVienTheoNhom_D> GetDs(string Ngay,int PageIndex,int PageSize)
        //{
        //    dbPMScontext db = new dbPMScontext();
        //    DateTime date = new DateTime();
        //    try
        //    {
        //        date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss",CultureInfo.InvariantCulture);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return db.HqLoaiNguyenLieuDs.Where(x=>x.MNgay > date).OrderByDescending(x=>x.MNgay).Skip((PageIndex -1)*PageSize).Take(PageSize).ToList();
        //}

        //public List<HQ_NhanVienTheoNhom_U> GetUs(string Ngay,int PageIndex,int PageSize)
        //{
        //    dbPMScontext db = new dbPMScontext();
        //    DateTime date = new DateTime();
        //    try
        //    {
        //        date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss",CultureInfo.InvariantCulture);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return db.HqLoaiNguyenLieuUs.Where(x=>x.MNgay > date).OrderByDescending(x=>x.MNgay).Skip((PageIndex -1)*PageSize).Take(PageSize).ToList();
        //}
        public int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.HQ_NhanVienTheoNhom();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(HQ_NhanVienTheoNhom item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<HQ_NhanVienTheoNhom>();
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
        private bool IsItemPass(HQ_NhanVienTheoNhom item)
        {
            return item != null && item.MaNhanVien != null && item.MaNhanVien.Trim() != "" &&
                   item.NgayGioBatDau != null;
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

            var items = Gets<HQ_NhanVienTheoNhom>();
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
        private void Reload_(ObservableRangeCollection<HQ_NhanVienTheoNhom> obj)
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

        public int Update<T>(T item)
        {
            var dao = new Dao.Repos.HQ.HQ_NhanVienTheoNhom();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(HQ_NhanVienTheoNhom item)
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
        public HQ_NhanVienTheoNhom? Find(long ma)
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

        public bool Exists(HQ_NhanVienTheoNhom item)
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
