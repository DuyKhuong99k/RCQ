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
using System.Windows.Input;
using Azure.Identity;
using System.Collections.Specialized;
using Vars.Hubs;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using System.Collections.ObjectModel;

namespace ViewModels.Repos.HQ
{
    public class PhieuSanLuongViewModel
    {
        private static PhieuSanLuongViewModel instance;
        public static PhieuSanLuongViewModel Instance => instance ??= new PhieuSanLuongViewModel();
        public List<PhieuSanLuongRaCoiTinhLuongXepKhuon> Gets(DateTime dateTime, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuSanLuongRaCoiTinhLuongXepKhuon();
                return dao.Gets(dateTime, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<PhieuSanLuongRaCoiTinhLuongXepKhuon> GetLuotRaCoi(
            DateTime dateTime,
            string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuSanLuongRaCoiTinhLuongXepKhuon();
                return dao.GetLuotRaCoi<PhieuSanLuongRaCoiTinhLuongXepKhuon>(dateTime, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Delete(DateTime dateTime, string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuSanLuongRaCoiTinhLuongXepKhuon();
                return dao.Delete(dateTime, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Insert(List<PhieuSanLuongRaCoiTinhLuongXepKhuon> items)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuSanLuongRaCoiTinhLuongXepKhuon();
                return dao.Insert(items);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Update(List<PhieuSanLuongRaCoiTinhLuongXepKhuon> items)
        {
            try
            {
                var dao = new Dao.Repos.HQ.PhieuSanLuongRaCoiTinhLuongXepKhuon();
                return dao.Update(items);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<T> GetsFullField<T>(DateTime dateTime, string xuongId)
        {
            var dao = new Dao.Repos.HQ.PhieuSanLuongRaCoiTinhLuongXepKhuon();
            return dao.GetsFullField<T>(dateTime, xuongId);
        }

        public List<PhieuSanLuongRaCoiTinhLuongXepKhuon> CommandReload(DateTime dateTime, string xuongId)
        {
            try
            {
                //PhieuSanLuongs = new ObservableCollection<PhieuSanLuongRaCoiTinhLuongXepKhuon>(
                //    Gets(
                //        AppViewModel.Ins.DateTimeNow,
                //        MainViewModel.XuongIdShared));

                var PhieuSanLuongs = new List<PhieuSanLuongRaCoiTinhLuongXepKhuon>(Gets(dateTime, xuongId));
                var phieuSanLuongs = GetLuotRaCoi(dateTime, xuongId);
                var tl = PhieuSanLuongs.Sum(x => x.TrongLuong);
                var tl1 = phieuSanLuongs.Sum(x => x.TrongLuong);
                if (!PhieuSanLuongs.Any())
                {
                    if (phieuSanLuongs.Any())
                    {
                        for (int i = 0; i < phieuSanLuongs.Count; i++)
                        {
                            var phieuSanLuong = new PhieuSanLuongRaCoiTinhLuongXepKhuon()
                            {
                                STT = i + 1,
                                Gio = DateTime.Now.TimeOfDay,
                                GioRaCoi = phieuSanLuongs[i].GioRaCoi,
                                MaCoi = phieuSanLuongs[i].MaCoi,
                                LuotRaCoi = phieuSanLuongs[i].LuotRaCoi,
                                MaMayCan = AppViewModel.Instance.PCName,
                                MaUserCan = AppViewModels.AppViewModel.Instance.UserName,
                                MaXuong = xuongId,
                                Ngay = dateTime.Date,
                                NgayThem = DateTime.Now.Date,
                                TrongLuong = phieuSanLuongs[i].TrongLuong,
                            };
                            PhieuSanLuongs.Add(phieuSanLuong);
                        }
                    }
                }
                else
                {
                    if (phieuSanLuongs.Any())
                    {
                        foreach (var phieuSanLuong in PhieuSanLuongs)
                        {
                            var phieu = phieuSanLuongs.SingleOrDefault(
                                x => x.LuotRaCoi == phieuSanLuong.LuotRaCoi &&
                                     x.MaCoi == phieuSanLuong.MaCoi);
                            if (phieu != null)
                            {
                                phieu.Gio = DateTime.Now.TimeOfDay;
                                phieu.GioRaCoi = phieuSanLuong.GioRaCoi;
                                phieu.MaCoi = phieuSanLuong.MaCoi;
                                phieu.LuotRaCoi = phieuSanLuong.LuotRaCoi;
                                phieu.MaMayCan = AppViewModel.Instance.PCName;
                                phieu.MaUserCan = AppViewModel.Instance.UserName;
                                phieu.MaXuong = xuongId;
                                phieu.NgayThem = DateTime.Now.Date;
                                phieu.MaNhom = phieuSanLuong.MaNhom;
                                phieu.Ngay = dateTime.Date;
                            }
                        }

                        PhieuSanLuongs.Clear();
                        for (int i = 0; i < phieuSanLuongs.Count; i++)
                        {
                            var phieuSanLuong = new PhieuSanLuongRaCoiTinhLuongXepKhuon()
                            {
                                STT = i + 1,
                                Gio = DateTime.Now.TimeOfDay,
                                GioRaCoi = phieuSanLuongs[i].GioRaCoi,
                                MaCoi = phieuSanLuongs[i].MaCoi,
                                LuotRaCoi = phieuSanLuongs[i].LuotRaCoi,
                                MaMayCan = AppViewModel.Instance.PCName,
                                MaUserCan = AppViewModel.Instance.UserName,
                                MaXuong = xuongId,
                                Ngay = dateTime.Date,
                                NgayThem = DateTime.Now.Date,
                                TrongLuong = phieuSanLuongs[i].TrongLuong,
                                MaNhom = phieuSanLuongs[i]?.MaNhom
                            };
                            PhieuSanLuongs.Add(phieuSanLuong);
                        }
                    }
                }
                return PhieuSanLuongs.ToList();
                //MessageBox.Show("Thực Hiện Xong");
            }
            catch (Exception exception)
            {
                return new List<PhieuSanLuongRaCoiTinhLuongXepKhuon>();
                //MessageBox.Show(exception.Message);
                //throw;
            }
        }
    }
}
