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

namespace ViewModels.Repos.HQ
{
    public partial class RolePermistionViewModel : ObservableObject
    {
        private static RolePermistionViewModel instance;
        [ObservableProperty] private bool idItemIsReadOnly = true;
        [ObservableProperty] private bool isAdd;
        [ObservableProperty] private bool isEdit;
        [ObservableProperty] private RolePermistion? item;
        [ObservableProperty] private ObservableRangeCollection<RolePermistion> items = new();
        [ObservableProperty] private ObservableRangeCollection<RolePermistion> userItems = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsVailSelectedItem))] private RolePermistion? selectedItem;
        [ObservableProperty] private ObservableRangeCollection<object> selectedItems = new();
        [ObservableProperty] private ICommand _closeItemWindowCommand;
        [ObservableProperty] private bool _isWindowItemShown = false;
        private readonly SynchronizationContext synchronizationContext;

        private RolePermistionViewModel()
        {
            try
            {
                Reload();
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                ////throw;
                //VmMessage.SetExceptionCommand.Execute(e);
            }
        }

        public static RolePermistionViewModel Instance => instance ??= new RolePermistionViewModel();

        public bool IsVailSelectedItem => SelectedItem != null;

        private AppViewModel VmApp => AppViewModel.Instance;

        private MessageViewModel VmMessage => MessageViewModel.Instance;

        public List<T> GetsFullField<T>(string? connStr = null, bool isServer = false)
        {
            var dao = new Dao.Repos.HQ.RolePermistion(connStr);
            return dao.GetsFullField<T>();
        }

        public List<T> GetsFullField<T>(int roleId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.RolePermistion(connStr);
            return dao.GetsFullField<T>(roleId);
        }



        private List<T> Gets<T>(string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.RolePermistion(connStr);
            return dao.Gets<T>();
        }
        private List<T> GetsLastAll<T>(string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.RolePermistion(connStr);
            return dao.GetsLastAll<T>();
        }

        /// <summary>
        /// Lấy toàn bộ danh sách rolepermistion
        /// </summary>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public List<RolePermistion> GetAllRolePermissions(string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.RolePermistion(connStr);
            return dao.GetAllRolePermissions();
        }
        /// <summary>
        /// Lấy danh sach rolepermistion theo Id
        /// </summary>
        /// <param name="permistionId"></param>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public RolePermistion GetAllRolePermistionById(int permistionId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.RolePermistion(connStr);
            return dao.GetRolePermistionById(permistionId);
        }
        /// <summary>
        /// lấy danh sách rolepermistion theo roleId
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public List<RolePermistion> GetAllRolePermistionByRoleId(int roleId, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.RolePermistion(connStr);
            return dao.GetAllRolePermistionByRoleId<RolePermistion>(roleId);
        }
        public List<RolePermistion> GetRolePermissions(string fu, string func, string? connStr = null)
        {
            var dao = new Dao.Repos.HQ.RolePermistion(connStr);
            return dao.GetRolePermissions(fu, func);
        }
        public List<RolePermistion> GetRolePermissions(string fu, string func, object context = null)
        {
            var dao = new Dao.Repos.HQ.RolePermistion(context);
            return dao.GetRolePermissions(fu, func);
        }


        public int Insert<T>(T item, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.RolePermistion(connStr);
                return dao.Insert<T>(item);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Insert(List<RolePermistion> items, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.RolePermistion(connStr);
                var rl = dao.Insert(items);
                if (rl > 0)
                {
                    lock (Items)
                    {
                        foreach (var item in items)
                        {
                            Items.Add(item);
                        }
                    }
                }


                return rl;

            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Update(RolePermistion item, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.RolePermistion(connStr);
                var rl = dao.Update(item);
                if (rl > 0)
                {
                    lock (Items)
                    {
                        var _item = Items.FirstOrDefault(x => x.Id == item.Id);
                        if (_item != null)
                        {
                            Items.Remove(_item);
                            Items.Add(item);
                        }
                    }
                }
                return rl;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Delete(RolePermistion item, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.RolePermistion(connStr);
                var rl = dao.Delete(item);
                if (rl > 0)
                {
                    lock (Items)
                    {
                        var _item = Items.FirstOrDefault(x => x.Id == item.Id);
                        if (_item != null)
                        {
                            Items.Remove(_item);
                        }
                    }
                }
                return rl;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Lấy toàn bộ danh sách role pẻmistion theo list roleid
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public List<RolePermistion> GetRolePermistions(List<int> roleIds)
        {
            var items = Items.Where(x => roleIds.Contains(x.RoleId)).ToList();
            return items;
        }

        private void Reload()
        {
            var items = GetsLastAll<RolePermistion>(Base.Ins.ConnectionString2);
            lock (Items)
            {
                Items.Clear();
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
        }
    }
}
