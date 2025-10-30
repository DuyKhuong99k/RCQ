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
    public partial class TheViewModel : ObservableObject
    {
        private static TheViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private TheTu? item;
        [ObservableProperty] private ObservableRangeCollection<TheTu> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private TheTu? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private TheViewModel()
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

        public static TheViewModel Instance => instance ??= new TheViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public TheTu CopyItem(TheTu item)
        {
            return new TheTu
            {
                MaTheTu = item.MaTheTu,
                MaNhanVien = item?.MaNhanVien,
                NgayGio = item.NgayGio,
                PCName =item?.PCName
            };
        }
        public TheTu CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public TheTu CreateDefaultNew()
        {
            
            return new TheTu
            {
                
                NgayGio = VmApp.DateTimeNow,
                PCName = VmApp.PCName
            };
        }

        public int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.TheTu();
            return dao.Delete(item);
        }
        public int Delete<T>(List<T> items)
        {
            var dao = new Dao.Repos.HQ.TheTu();
            return dao.Delete(items);
        }
        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(TheTu item)
        {
            try
            {
                if (Delete(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.MaTheTu == item.MaTheTu);
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
            var dao = new Dao.Repos.HQ.TheTu();
            return dao.Gets<T>();
        }
        public List<string> GetDs(string Ngay,int PageIndex,int PageSize)
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
            return db.HqTheTuDs.Where(x=>x.Ngay > date).OrderByDescending(x=>x.Ngay).Skip((PageIndex -1)*PageSize).Take(PageSize).Select(x=>x.MaTheTu).ToList();
        }
        public int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.TheTu();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(TheTu item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<TheTu>();
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
        private bool IsItemPass(TheTu item)
        {
            return item != null && item.MaTheTu != null && item.MaTheTu.Trim() != "" &&
                   item.MaNhanVien != null && item.MaNhanVien.Trim() != "";
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

            var items = Gets<TheTu>();
            if (items.Any())
                lock (Items)
                {
                    try
                    {
                        Items.AddRange(items);
                        //foreach (var item in items)
                        //{
                        //    Items.Add(item);
                        //}

                      
                    }
                    catch (NotSupportedException e)
                    {

                    }

                }


        }

        [RelayCommand]
        private void Reload_(ObservableRangeCollection<TheTu> obj)
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
            var dao = new Dao.Repos.HQ.TheTu();
            return dao.Update(item);
        }

        public int Update<T>(List<T> items)
        {
            var dao = new Dao.Repos.HQ.TheTu();
            return dao.Update(items);
        }
        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(TheTu item)
        {
            try
            {
                if (Update(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.MaTheTu == item.MaTheTu);
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
                        var _item = Items.SingleOrDefault(x => x.MaTheTu == Item.MaTheTu);
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
        public TheTu? Find(string ma)
        {
            try
            {
                return Items.FirstOrDefault(x => x.MaTheTu == ma);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Exists(TheTu item)
        {
            try
            {

                return Find(item.MaTheTu) != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
