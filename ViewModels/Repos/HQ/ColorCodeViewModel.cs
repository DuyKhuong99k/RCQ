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
    public partial class ColorCodeViewModel : ObservableObject
    {
        private static ColorCodeViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private ColorCode? item;
        [ObservableProperty] private ObservableRangeCollection<ColorCode> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private ColorCode? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private ColorCodeViewModel()
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

        public static ColorCodeViewModel Instance => instance ??= new ColorCodeViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public ColorCode CopyItem(ColorCode item)
        {
            return new ColorCode
            {
                Code = item.Code,
                Code2 = item.Code2,
                Name = item.Name,
                ColorRGB = item.ColorRGB
            };
        }
        public ColorCode CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public ColorCode CreateDefaultNew()
        {
            //var maxId = Items.Where(x => int.TryParse(x.Code, out var rl))
            //       .Select(x => int.Parse(x.Code))
            //       .DefaultIfEmpty(0)
            //       .Max();
            //var id = $"{(maxId + 1).ToString()}";
            return new ColorCode
            {
                //Code = id
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.ColorCode();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(ColorCode item)
        {
            try
            {
                if (Delete(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.Code == item.Code);
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
            var dao = new Dao.Repos.HQ.ColorCode();
            return dao.Gets<T>();
        }
        public List<HQ_ColorCode_U> GetUs(string Ngay,int PageIndex,int PageSize)
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
            return db.HqColorCodeUs.Where(x=>x.Ngay > date).OrderByDescending(x=>x.MNgay).Skip((PageIndex -1)*PageSize).Take(PageSize).ToList();
        }
        public List<HQ_ColorCode_D> GetDs(string Ngay,int PageIndex,int PageSize)
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
            return db.HqColorCodeDs.Where(x=>x.Ngay > date).OrderByDescending(x=>x.MNgay).Skip((PageIndex -1)*PageSize).Take(PageSize).ToList();
        }
        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.ColorCode();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(ColorCode item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<ColorCode>();
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
        private bool IsItemPass(ColorCode item)
        {
            return item != null && item.Name != null && item.Name.Trim() != "" &&
                   item.Code != null && item.Code.Trim() != "";
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

            var items = Gets<ColorCode>();
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
        private void Reload_(ObservableRangeCollection<ColorCode> obj)
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
            var dao = new Dao.Repos.HQ.ColorCode();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(ColorCode item)
        {
            try
            {
                if (Update(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.Code == item.Code);
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
                        var _item = Items.SingleOrDefault(x => x.Code == Item.Code);
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
        public ColorCode? Find(string ma)
        {
            try
            {
                return Items.FirstOrDefault(x => x.Code == ma);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Exists(ColorCode item)
        {
            try
            {

                return Find(item.Code) != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
