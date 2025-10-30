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
    public partial class TheRoViewModel: ObservableObject
    {
        private static TheRoViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private TheRo? item;
        [ObservableProperty] private ObservableRangeCollection<TheRo> items = new();

        [ObservableProperty][NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private TheRo? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;
        private TheRoViewModel()
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

        public static TheRoViewModel Instance => instance ??= new TheRoViewModel();

        private AppViewModel VmApp => AppViewModel.Instance;
        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public TheRo CopyItem(TheRo item)
        {
            return new TheRo
            {
                ColorCode = item.ColorCode,
                CreatedDateTime = item.CreatedDateTime,
                MaThe = item.MaThe,
                IsRach = item.IsRach,
                MaThanhPhamDinhHinh = item.MaThanhPhamDinhHinh,
            };
        }
        public TheRo CopySelectedItem()
        {
            return CopyItem(SelectedItem);
        }

        public TheRo CreateDefaultNew()
        {
           
            return new TheRo
            {
                CreatedDateTime = DateTime.Now
            };
        }

        public int Delete<T>(T item)
        {
            try
            {
                dbPMScontext db = new dbPMScontext();
                if (item is TheRo the)
                {
                    var _the = new HQ_TheRo_D()
                    {
                        MaThe = the.MaThe,
                        Ngay = DateTime.Now,
                        CreatedDateTime = the.CreatedDateTime,
                    
                    };
                    db.HqTheRoDs.Add(_the);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }
            
            var dao = new Dao.Repos.HQ.TheRo();
            return dao.Delete(item);
        }
        public int Delete<T>(List<T> items)
        {
            try
            {
                dbPMScontext db = new dbPMScontext();
                
                if (items is  List<TheRo> thes)
                {
                    foreach (var the in thes)
                    {
                        var _the = new HQ_TheRo_D()
                        {
                            MaThe = the.MaThe,
                            Ngay = DateTime.Now,
                            CreatedDateTime = the.CreatedDateTime,
                    
                        };
                        db.HqTheRoDs.Add(_the);

                    }
                    db.SaveChanges();
                }
               
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }
            
            var dao = new Dao.Repos.HQ.TheRo();
            return dao.Delete(items);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Delete_(TheRo item)
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
            var dao = new Dao.Repos.HQ.TheRo();
            return dao.Gets<T>();
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
            return db.HqTheRoDs.Where(x=>x.Ngay.Date >= date.Date).OrderByDescending(x=>x.Ngay).Skip((PageIndex -1)*PageSize).Take(PageSize).Select(x=>x.MaThe).ToList();
        }
        public int Insert<T>(T item)
        {
            var dao = new Dao.Repos.HQ.TheRo();
            return dao.Insert(item);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Insert_(TheRo item)
        {
            try
            {
                if (Insert(item) > 0)
                {
                    var items = new List<TheRo>();
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
        private bool IsItemPass(TheRo item)
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

            var items = Gets<TheRo>();
            if (items.Any())
                lock (Items)
                {
                    try
                    {
                        Items.AddRange(items);
                        

                     
                    }
                    catch (NotSupportedException e)
                    {

                    }

                }


        }

        [RelayCommand]
        private void Reload_(ObservableRangeCollection<TheRo> obj)
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
            var dao = new Dao.Repos.HQ.TheRo();
            return dao.Update(item);
        }

        public int Update<T>(List<T> items)
        {
            dbPMScontext db = new dbPMScontext();
            if (items is  List<TheRo> thes)
            {
                //the.Ngay = DateTime.Now.Date;
                //db.HqTheThanhPhamDs.Add(the);
                //db.SaveChanges();
                foreach (var theRo in thes)
                {
                    var the = new HQ_TheRo_U()
                    {
                       MaThe = theRo.MaThe,
                       Ngay = DateTime.Now,
                       CreatedDateTime = theRo.CreatedDateTime
                        
                    };
                    db.HqTheRoUs.Add(the);

                }
                db.SaveChanges();
            }
            var dao = new Dao.Repos.HQ.TheRo();
            return dao.Update(items);
        }

        [RelayCommand(CanExecute = nameof(IsItemPass))]
        private void Update_(TheRo item)
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
        public TheRo? Find(string ma)
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

        public bool Exists(TheRo item)
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
