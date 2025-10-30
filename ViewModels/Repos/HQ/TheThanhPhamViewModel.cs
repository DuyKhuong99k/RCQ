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
using Vars;

namespace ViewModels.Repos.HQ
{
    public partial class TheThanhPhamViewModel : ObservableObject
    {
        private static TheThanhPhamViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private TheThanhPham? item;
        [ObservableProperty] private ObservableRangeCollection<TheThanhPham> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private TheThanhPham? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private TheThanhPhamViewModel()
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

        public static TheThanhPhamViewModel Instance => instance ??= new TheThanhPhamViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public TheThanhPham CopyItem(TheThanhPham item)
        {
            return new TheThanhPham
            {
                MaThanhPham = item.MaThanhPham,
                MaThe = item.MaThe,
                MaSizeDinhHinh = item.MaSizeDinhHinh,
                TrongLuongTare = item.TrongLuongTare,
                MaMayLangDa = item.MaMayLangDa,
                MaThanhPhamFillet = item.MaThanhPhamFillet,
                MaThanhPhamPhuPham = item.MaThanhPhamPhuPham,
                MaSizeFillet = item.MaSizeFillet,
                IsZero = item.IsZero,
                IsRestart = item.IsRestart,
                MaQuyTrinhT = item.MaQuyTrinhT,
                MaLoaiNguyenLieu = item.MaLoaiNguyenLieu,
                STTPhieuPhanCoChiTietT = item.STTPhieuPhanCoChiTietT,
                NgayGio = item.NgayGio,
                MaSize = item.MaSize,
                CoiXepKhuonName = item.CoiXepKhuonName,
                MaCoiXepKhuon = item.MaCoiXepKhuon,
                QuyTrinhName = item.QuyTrinhName,
                SizeDHName = item.SizeDHName,
                SizeFilletName = item.SizeFilletName,
                ThanhPhamDHName = item.ThanhPhamDHName,
                ThanhPhamFilletName = item.ThanhPhamFilletName,
                ThanhPhamPhuPhamName = item.ThanhPhamPhuPhamName,
                MaThanhPhamChinhXepKhuon = item.MaThanhPhamChinhXepKhuon,
                MaSizeChinhXepKhuon = item.MaSizeChinhXepKhuon,
                MaThanhPhamPhuXepKhuon = item.MaThanhPhamPhuXepKhuon,
                MaSizePhuXepKhuon = item.MaSizePhuXepKhuon,
                MaChatLuongChinhXepKhuon = item.MaChatLuongChinhXepKhuon,
                LoLevel = item.LoLevel,
                ThanhPhamChinhXepKhuonName = item.ThanhPhamChinhXepKhuonName,
                SizeChinhXepKhuonName = item.SizeChinhXepKhuonName,
                ChatLuongChinhXepKhuonName = item.ChatLuongChinhXepKhuonName,
                ThanhPhamPhuXepKhuonName = item.ThanhPhamPhuXepKhuonName,
                SizePhuXepKhuonName = item.SizePhuXepKhuonName,
                MaChieuXa = item.MaChieuXa,
                ChieuXaName = item.ChieuXaName,
                ChatLuongPhuXepKhuonName = item.ChatLuongPhuXepKhuonName,
                LoaiNguyenLieuName = item.LoaiNguyenLieuName,
                MaChatLuongPhuXepKhuon = item.MaChatLuongPhuXepKhuon,
                SizeName = item.SizeName,
                ThanhPhamName = item.ThanhPhamName
            };
        }
        public TheThanhPham CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public TheThanhPham CreateDefaultNew()
        {
            return new TheThanhPham
            {
                TrongLuongTare = 0,
                IsZero = false,
                IsRestart = false,
                NgayGio = DateTime.Now
            };
        }

        public int Delete<T>(T item)
        {
            try
            {
                dbPMScontext db = new dbPMScontext();
                if (item is HQ_TheThanhPham_D the)
                {
                    the.Ngay = DateTime.Now.Date;
                    db.HqTheThanhPhamDs.Add(the);
                    db.SaveChanges();
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }
            var dao = new Dao.Repos.HQ.TheThanhPham();
            return dao.Delete(item);
        }
        public int Delete<T>(List< T> items)
        {
            try
            {
                dbPMScontext db = new dbPMScontext();
                if (items is  List<TheThanhPham> thes)
                {
                    //the.Ngay = DateTime.Now.Date;
                    //db.HqTheThanhPhamDs.Add(the);
                    //db.SaveChanges();
                    foreach (var theTu in thes)
                    {
                        var the = new HQ_TheThanhPham_D()
                        {
                      
                            Ngay = DateTime.Now,
                            MaThe = theTu.MaThe,
                            
                        };
                        db.HqTheThanhPhamDs.Add(the);

                    }
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }
            var dao = new Dao.Repos.HQ.TheThanhPham();
            return dao.Delete(items);
        }
        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(TheThanhPham item)
        {
            try
            {
                if (Delete(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.MaThe == item.MaThe);
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
            var dao = new Dao.Repos.HQ.TheThanhPham();
            return dao.Gets<T>();
        }
        private List<T> GetHqs<T>()
        {
            var dao = new Dao.Repos.HQ.TheThanhPham();
            return dao.GetHqs<T>();
        }
        public List<string> GetDs(string Ngay,int PageIndex,int PageSize)
        {
            dbPMScontext db = new dbPMScontext();
            var date = new DateTime();
            var index = Ngay.IndexOf("=", StringComparison.Ordinal);
            if (index == -1)
            {
                try
                {
                    date = DateTime.ParseExact(Ngay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
            }
            else
            {
                var _data = Ngay.Substring(index + 1).Split(',');
                var mayCanId = _data.FirstOrDefault() ?? "";
                var mNgay = _data.LastOrDefault() ?? "";
                try
                {
                    date = DateTime.ParseExact(mNgay, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    //throw ex;
                }
                
            }
            return db.HqTheThanhPhamDs.Where(x=>x.Ngay.Date >= date.Date).OrderByDescending(x=>x.Ngay).Skip((PageIndex -1)*PageSize).Take(PageSize).Select(x=>x.MaThe).ToList();
        }

        public int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.TheThanhPham();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(TheThanhPham item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<TheThanhPham>();
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
        private bool IsItemPass(TheThanhPham item)
        {
            return item != null && item.MaThe != null && item.MaThe.Trim() != ""
                  ;
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

            if (VmApp.ComName == nameof(ComNames.RCQTG))
            {
                var items = GetHqs<TheThanhPham>();
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
            else
            {
                var items = Gets<TheThanhPham>();
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


        }

        [RelayCommand]
        private void Reload_(ObservableRangeCollection<TheThanhPham> obj)
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

            var dao = new Dao.Repos.HQ.TheThanhPham();
            return dao.Update(item);
        }
        public int Update<T>(List<T> items)
        {
            try
            {
                dbPMScontext db = new dbPMScontext();
                if (items is  List<TheThanhPham> thes)
                {
                    //the.Ngay = DateTime.Now.Date;
                    //db.HqTheThanhPhamDs.Add(the);
                    //db.SaveChanges();
                    foreach (var theTu in thes)
                    {
                        var the = new HQ_TheThanhPham_D()
                        {
                      
                            Ngay = DateTime.Now,
                            MaThe = theTu.MaThe,
                            
                        };
                        db.HqTheThanhPhamDs.Add(the);

                    }
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }
            var dao = new Dao.Repos.HQ.TheThanhPham();
            return dao.Update(items);
        }
        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(TheThanhPham item)
        {
            try
            {
                if (Update(item) > 0)
                    lock (Items)
                    {
                        var _item = Items.SingleOrDefault(x => x.MaThe == item.MaThe);
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
                        var _item = Items.SingleOrDefault(x => x.MaThe == Item.MaThe);
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
        public TheThanhPham? Find(string ma)
        {
            try
            {
                return Items.FirstOrDefault(x => x.MaThe == ma);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool Exists(TheThanhPham item)
        {
            try
            {

                return Find(item.MaThe) != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
