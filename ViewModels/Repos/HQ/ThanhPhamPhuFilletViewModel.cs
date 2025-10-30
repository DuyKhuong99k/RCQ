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

namespace ViewModels.Repos.HQ
{
    public partial class ThanhPhamPhuFilletViewModel
    {
        private static ThanhPhamPhuFilletViewModel instance;
        public static ThanhPhamPhuFilletViewModel Instance => instance ??= new ThanhPhamPhuFilletViewModel();
        public List<CongViecPhuFillet> Get(string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.CongViecPhuFillet(connStr);
                return dao.GetCongViecPhuFillets();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<BravoModelV1.Model.PhanBoNhanVienPhu> ThanhPhamPhuFilletSelectChanged(DateTime dateTime, string xuongId, string thanhPham)
            {
                try
                {
                    //NhanVienViewModel.Ins.PhanBoNhanVienPhuFillets.Clear();
                    var phanBoNhanVienPhuFillets = new List<BravoModelV1.Model.PhanBoNhanVienPhu>();
                    var phanBos = NhanVienViewModel.Instance
                        .GetNhanVienPhanBoPhuFilletByLoaiThanhPhamPhu(thanhPham,dateTime,xuongId);
                    if (!phanBos.Any())
                    {
                        phanBos = NhanVienViewModel.Instance
                            .GetNhanVienPhanBoPhuFilletByLoaiThanhPhamPhu(thanhPham,new DateTime(2020, 03, 21),xuongId);
                        phanBos.All(
                            x =>
                            {
                                x.Ngay = dateTime.Date;
                                return true;
                            });
                        //MessageBox.Show("Đã tải danh sách cài đặt mặt định vui lòng nhấn lưu để xác nhận cho hôm nay");
                    }

                    if (phanBos.Any())
                    {
                        var items = new List<BravoModelV1.Model.PhanBoNhanVienPhu>();
                        foreach (var item in phanBos)
                        {
                            items.Add(
                                new BravoModelV1.Model.PhanBoNhanVienPhu()
                                {
                                    MaCongViec = item.MaCongViecPhuFillet,
                                    MaNhanVien = item.MaNhanVien,
                                    MaXuong = item.MaXuong,
                                    Ngay = dateTime.Date,
                                    TyLeHuong = item.TyLeHuong,
                                    TyLeTru = item.TyLeTru
                                });
                        }

                        phanBoNhanVienPhuFillets =new List<BravoModelV1.Model.PhanBoNhanVienPhu>(items);
                    }
                    return phanBoNhanVienPhuFillets.ToList();
                    //var nhanViens = NhanVienViewModel.Ins
                    //    .GetNhanVienByLoaiThanhPhamPhu(ThanhPhamPhuFilletSelectedItem.Ma,
                    //                                   MainViewModel.DateTimeNowShared,
                    //                                   MainViewModel.XuongIdShared);
                    //if(!nhanViens.Any())
                    //{
                    //    nhanViens = NhanVienViewModel.Ins
                    //        .GetNhanVienByLoaiThanhPhamPhu(ThanhPhamPhuFilletSelectedItem.Ma,
                    //                                       new DateTime(2020,03,21),
                    //                                       MainViewModel.XuongIdShared);
                    //    MessageBox.Show("Đã tải danh sách cài đặt mặt định vui lòng nhấn lưu để xác nhận cho hôm nay");
                    //}
                    //NhanVienViewModel.Ins.NhanViensTheoThanhPham = new ObservableCollection<NhanVienDaiThanh>(nhanViens);
                }
                catch (Exception exception)
                {
                    //MessageBox.Show(exception.Message);
                    //throw;
                    return new List<BravoModelV1.Model.PhanBoNhanVienPhu>();
                }
            }
    }
}
