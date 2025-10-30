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
    public partial class ThanhPhamSoCheDinhHinhViewModel : ObservableObject
    {
        private static ThanhPhamSoCheDinhHinhViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private MaThanhPhamSoCheDinhHinh? item;
        [ObservableProperty] private ObservableRangeCollection<MaThanhPhamSoCheDinhHinh> items = new();
        [ObservableProperty] private ObservableRangeCollection<MaThanhPhamSoCheDinhHinh> usedItems = new();
        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private MaThanhPhamSoCheDinhHinh? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private ThanhPhamSoCheDinhHinhViewModel()
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

        public static ThanhPhamSoCheDinhHinhViewModel Instance => instance ??= new ThanhPhamSoCheDinhHinhViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public MaThanhPhamSoCheDinhHinh CopyItem(MaThanhPhamSoCheDinhHinh item)
        {
            return new MaThanhPhamSoCheDinhHinh
            {
                Ma = item.Ma,
                SuDung = item.SuDung,
                Ten = item.Ten,
                Ban09 = item.Ban09,
                Nhan = item.Nhan,
                BatCO = item.BatCO,
                BravoId = item.BravoId,
                CaMuoi = item.CaMuoi,
                LoaiGui = item.LoaiGui,
                Max = item.Max,
                Min = item.Min,
                NguyenLieu = item.NguyenLieu,
                SauLangDa = item.SauLangDa,
                TinhGio = item.TinhGio,
                TinhKiem = item.TinhKiem,
                TinhPhucVu = item.TinhPhucVu,
                TruocLangDa = item.TruocLangDa,
                X = item.X,
                Y = item.Y,
                IsNhapTay = item.IsNhapTay,
                Createdby = item.Createdby,
                CreatedDateTime = item.CreatedDateTime,
                Modifiedby = item.Modifiedby,
                ModifiedDateTime = item.ModifiedDateTime,
            };
        }
        public MaThanhPhamSoCheDinhHinh CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public MaThanhPhamSoCheDinhHinh CreateDefaultNew()
        {
            var maxId = Items.Where(x => int.TryParse(x.Ma, out int rl)).Select(x => x.Ma).DefaultIfEmpty("0")
                   .Max();
            var id = (int.Parse(maxId) + 1).ToString("000");
            return new MaThanhPhamSoCheDinhHinh()
            {
                SuDung = true,
                Ma = id,
                Nhan = true,
                Max = 999,
                Min = 0,
                IsNhapTay = false,
                //Createdby = AppViewModel.Instance.UserName,
                CreatedDateTime = DateTime.Now
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.MaThanhPhamSoCheDinhHinh();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(MaThanhPhamSoCheDinhHinh item)
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
            var dao = new Dao.Repos.HQ.MaThanhPhamSoCheDinhHinh();
            return dao.Gets<T>();
        }

        private int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.MaThanhPhamSoCheDinhHinh();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(MaThanhPhamSoCheDinhHinh item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<MaThanhPhamSoCheDinhHinh>();
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
        private bool IsItemPass(MaThanhPhamSoCheDinhHinh item)
        {
            return item.Ma != null &&
                 item.Ma.Trim() != string.Empty &&
                 item.Ten != null &&
                 item.Ten.Trim() != string.Empty;
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

            var items = Gets<MaThanhPhamSoCheDinhHinh>();
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
        private void Reload_(ObservableRangeCollection<MaThanhPhamSoCheDinhHinh> obj)
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
            var dao = new Dao.Repos.HQ.MaThanhPhamSoCheDinhHinh();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(MaThanhPhamSoCheDinhHinh item)
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
        public MaThanhPhamSoCheDinhHinh? Find(string ma)
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

        public bool Exists(MaThanhPhamSoCheDinhHinh item)
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
