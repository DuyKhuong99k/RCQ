using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AppViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Models.Repos.Models;
using MvvmHelpers;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;

namespace ViewModels.Repos.HQ
{
   public partial class ThanhPhamPhuXepKhuon_ColorViewModel : ObservableObject
    {
        private static ThanhPhamPhuXepKhuon_ColorViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private MaThanhPhamXepKhuon_Color? item;
        [ObservableProperty] private ObservableRangeCollection<MaThanhPhamXepKhuon_Color> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private MaThanhPhamXepKhuon_Color? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private ThanhPhamPhuXepKhuon_ColorViewModel()
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

        public static ThanhPhamPhuXepKhuon_ColorViewModel Instance => instance ??= new ThanhPhamPhuXepKhuon_ColorViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public MaThanhPhamXepKhuon_Color CopyItem(MaThanhPhamXepKhuon_Color item)
        {
            return new MaThanhPhamXepKhuon_Color
            {
                ColorCode = item.ColorCode,
                MaLo = item.MaLo,
                MaThanhPham = item.MaThanhPham,
                MaXuong = item.MaXuong,
                Ngay = item.Ngay,
                
            };
        }
        public MaThanhPhamXepKhuon_Color CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public MaThanhPhamXepKhuon_Color CreateDefaultNew()
        {
           
            return new MaThanhPhamXepKhuon_Color
            {
                Ngay = AppViewModel.Instance.DateTimeNow.Date,
                MaXuong = XiNghiepViewModel.Instance.SelectedItem?.Ma
            };
        }

        private int Delete<T>(T item)
        {
            var dao = new Dao.Repos.HQ.MaThanhPhamXepKhuon_Color();
            return dao.Delete(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(MaThanhPhamXepKhuon_Color item)
        {
            try
            {
                if (Delete(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.MaThanhPham == item.MaThanhPham);
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

        public List<T> GetsLastDay<T>()
        {
            try
            {
                var dao = new Dao.Repos.HQ.MaThanhPhamXepKhuon_Color();
                return dao.GetsLastDay<T>();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.MaThanhPhamXepKhuon_Color();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(MaThanhPhamXepKhuon_Color item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<MaThanhPhamXepKhuon_Color>();
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
        private bool IsItemPass(MaThanhPhamXepKhuon_Color item)
        {
            return item != null &&
                 item.ColorCode != null &&
                 item.ColorCode.Trim() != string.Empty &&
                 item.Ngay != null &&
                 item.MaLo != null &&
                 item.MaLo.Trim() != string.Empty &&
                 item.MaXuong != null &&
                 item.MaXuong.Trim() != string.Empty;
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

            var items = GetsLastDay<MaThanhPhamXepKhuon_Color>();
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
        private void Reload_(ObservableRangeCollection<MaThanhPhamXepKhuon_Color> obj)
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
            var dao = new Dao.Repos.HQ.MaThanhPhamXepKhuon_Color();
            return dao.Update(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(MaThanhPhamXepKhuon_Color item)
        {
            try
            {
                if (Update(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.MaThanhPham == item.MaThanhPham);
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
                        var _item = Items.SingleOrDefault(x => x.MaThanhPham == Item.MaThanhPham);
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
        public MaThanhPhamXepKhuon_Color? Find(string ma)
        {
            try
            {
                return Items.FirstOrDefault(x => x.MaThanhPham == ma);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Exists(MaThanhPhamXepKhuon_Color item)
        {
            try
            {

                return Find(item.MaThanhPham) != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
