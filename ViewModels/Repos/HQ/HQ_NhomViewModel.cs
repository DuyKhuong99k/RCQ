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
    public partial class HQ_NhomViewModel : ObservableObject
    {
        private static HQ_NhomViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private HQ_Nhom? item;
        [ObservableProperty] private ObservableRangeCollection<HQ_Nhom> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private HQ_Nhom? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private HQ_NhomViewModel()
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

        public static HQ_NhomViewModel Instance => instance ??= new HQ_NhomViewModel();
        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;
        public HQ_Nhom CopyItem(HQ_Nhom item)
        {
            return new HQ_Nhom
            {
                SuDung = item.SuDung,
                Id = item?.Id,
                Ten = item?.Ten,
                //MNgay = item?.MNgay
            };
        }
        public HQ_Nhom CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public HQ_Nhom CreateDefaultNew()
        {
            var maxId = Items.Where(x => int.TryParse(x.Id, out var rl))
                   .Select(x => int.Parse(x.Id))
                   .DefaultIfEmpty(0)
                   .Max();
            var id = $"{(maxId + 1).ToString()}";
            return new HQ_Nhom
            {
                SuDung = true,
                Id = id
            };
        }

        public int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.HQ_Nhom();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(HQ_Nhom item)
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
            var dao = new Dao.Repos.HQ.HQ_Nhom();
            return dao.Gets<T>();
        }
        public HQ_Nhom GetByMa(string id)
        {
            dbPMScontext db = new dbPMScontext();
            var item = db.HqNhoms.FirstOrDefault(x => x.Id == id);
            return item;
        }
        //public List<HQ_Nhom_D> GetDs(string Ngay,int PageIndex,int PageSize)
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

        //public List<HQ_Nhom_U> GetUs(string Ngay,int PageIndex,int PageSize)
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
            var dao = new Dao.Repos.HQ.HQ_Nhom();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(HQ_Nhom item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<HQ_Nhom>();
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
        private bool IsItemPass(HQ_Nhom item)
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
            }

            var items = Gets<HQ_Nhom>();
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
        private void Reload_(ObservableRangeCollection<HQ_Nhom> obj)
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
            var dao = new Dao.Repos.HQ.HQ_Nhom();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(HQ_Nhom item)
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
        public HQ_Nhom? Find(string ma)
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

        public bool Exists(HQ_Nhom item)
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
