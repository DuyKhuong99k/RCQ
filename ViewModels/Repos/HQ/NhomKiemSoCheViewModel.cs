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
using Vars.Hubs;
using Microsoft.Data.SqlClient;

namespace ViewModels.Repos.HQ
{
    public partial class NhomKiemSoCheViewModel : ObservableObject
    {
        private static NhomKiemSoCheViewModel instance;
        public static NhomKiemSoCheViewModel Instance => instance ??= new NhomKiemSoCheViewModel();
        public List<NhomKiemSoChe> Gets(string xuongId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.NhomKiemSoChe(connStr);
                return dao.Gets(xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<NhanVienDaiThanh> NhomKiemSoCheSelectionChanged(DateTime dateTime, string xuongId, string nhomKiemSoChe)
        {
            try
            {
                Task.Delay(1000);
                var nhanVienNhoms = new List<NhanVienDaiThanh>();

                var botris = BoTriNhomKiemSoCheViewModel.Instance.GetWithNhoms(dateTime, xuongId, nhomKiemSoChe);
                if (botris == null || !botris.Any())
                {
                    botris = BoTriNhomKiemSoCheViewModel.Instance.GetWithNhoms(new DateTime(2020, 03, 21), xuongId, nhomKiemSoChe);
                    if (botris != null && botris.Any())
                        botris.All(
                            x =>
                            {
                                x.Ngay = dateTime.Date;
                                return true;
                            });
                }

                if (botris != null && botris.Any())
                {
                    var ids = botris.Select(x => x.MaNhanVien).Distinct();
                    nhanVienNhoms = new List<NhanVienDaiThanh>(NhanVienViewModel.Instance.GetNhanVienDaiThanhs(ids));
                }
                return nhanVienNhoms.ToList();
            }
            catch (Exception exception)
            {
                return new List<NhanVienDaiThanh>();
                //throw;
            }
        }
    }
}
