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
using BravoModelV1.Model;
using Models.Repos.AppModel;
using System.Collections.ObjectModel;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace ViewModels.Repos.HQ
{
    public partial class NhomSoCheDinhHinhViewModel : ObservableObject
    {
        private static NhomSoCheDinhHinhViewModel instance;
        public static NhomSoCheDinhHinhViewModel Instance => instance ??= new NhomSoCheDinhHinhViewModel();
        //[ObservableProperty] private ObservableCollection<NhomSoCheDinhHinh> _nhomSoCheDinhHinhs = new();
        private NhomSoCheDinhHinhViewModel()
        {
            try
            {
                //NhomSoCheDinhHinhs = new ObservableCollection<NhomSoCheDinhHinh>(Gets());
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<NhomSoCheDinhHinh> Gets(string xuongId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.NhomSoCheDinhHinh(connStr);
                var items = dao.Gets(xuongId);
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public List<NhomSoCheDinhHinh> Get
        public List<NhomSoCheDinhHinh> Gets(string xuongId, bool isChinh, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.NhomSoCheDinhHinh(connStr);
                return dao.Gets(xuongId, isChinh);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public List<NhanVienDaiThanh> NhomSoCheSelectionChanged(DateTime dateTime, string xuongId, string nhomSoChe)
        {
            try
            {
                //PhieuCanViewModel.Ins.IsBusy = true;
                Task.Delay(1000);
                var nhanVienNhom = new List<NhanVienDaiThanh>();
                var botris = BoTriNhomSoCheViewModel.Instance.GetWithNhomSoChes(dateTime, xuongId, nhomSoChe);
                if (botris == null || !botris.Any())
                {
                    botris = BoTriNhomSoCheViewModel.Instance.GetWithNhomSoChes(new DateTime(2020, 03, 21), xuongId, nhomSoChe);
                    if (botris != null && botris.Any())
                        botris.All(
                            x =>
                            {
                                x.Ngay = dateTime.Date;
                                return true;
                            });
                    //foreach (var item in botris)
                    //{
                    //    BoTriNhomSoCheViewModel.Instance.Insert(item);
                    //}
                }

                if (botris != null && botris.Any())
                {
                    var ids = botris.Select(x => x.MaNhanVien).Distinct();
                    nhanVienNhom = new List<NhanVienDaiThanh>(
                        NhanVienViewModel.Instance.GetNhanVienDaiThanhs(ids));
                }
                return nhanVienNhom.ToList();
            }
            catch (Exception exception)
            {
                //MessageBox.Show(exception.Message);
                return new List<NhanVienDaiThanh>();
                //throw;
            }
        }
    }
}
