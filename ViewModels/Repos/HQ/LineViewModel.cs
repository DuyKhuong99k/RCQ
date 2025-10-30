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
    public partial class LineViewModel
    {
        private static LineViewModel instance;
        public static LineViewModel Instance => instance ??= new LineViewModel();
        //[ObservableProperty] private ObservableRangeCollection<string> employeeCodes = new();
        public List<NhanVienPhuTheoLine> LineSelectChangedCommand(DateTime dateTime, string xuongId, string maLine, string maThanhPham)
        {
            try
            {
                var nhanViens = NhanVienViewModel.Instance
                    .GetNhanViensPhuByLine(maLine, dateTime, xuongId, maThanhPham);

                if (!nhanViens.Any())
                {
                    nhanViens = NhanVienViewModel.Instance
                        .GetNhanViensPhuByLine(maLine, new DateTime(2020, 03, 21), xuongId, maThanhPham);
                    //MessageBox.Show("Đã tải danh sách cài đặt mặt định vui lòng nhấn lưu để xác nhận cho hôm nay");
                }

                nhanViens.All(
                    x =>
                    {
                        x.Ngay = dateTime;
                        return true;
                    });
                var nhanVienPhuTheoLines = new List<NhanVienPhuTheoLine>(nhanViens);
                return nhanVienPhuTheoLines.ToList();
            }
            catch (Exception exception)
            {
                return new List<NhanVienPhuTheoLine>();
                //MessageBox.Show(exception.Message);
                //throw;
            }
        }
    }
}
