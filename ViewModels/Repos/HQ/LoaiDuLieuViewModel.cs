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
using System.Collections.ObjectModel;
using BravoModelV1.Model;

namespace ViewModels.Repos.HQ
{
    public partial class LoaiDuLieuViewModel : ObservableObject
    {
        private static LoaiDuLieuViewModel instance;

        [ObservableProperty] private ObservableCollection<BravoModelV1.Model.LoaiDuLieu> _loaiDuLieus = new ObservableCollection<BravoModelV1.Model.LoaiDuLieu>();

        private LoaiDuLieuViewModel()
        {
            try
            {
                //Reload();
                LoaiDuLieus = new ObservableCollection<LoaiDuLieu>(Gets());

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
                // VmMessage.SetExceptionCommand.Execute(e);
            }
        }
        public List<LoaiDuLieu> Gets()
        {
            try
            {
                var items = new List<LoaiDuLieu>()
                {
                    new LoaiDuLieu()
                    {
                        Id = 1,
                        Ten = "Bảng"
                    },
                    new LoaiDuLieu()
                    {
                        Id = 2,
                        Ten = "Nhập"
                    },
                    new LoaiDuLieu()
                    {
                        Id = 3,
                        Ten = "Nhập và Bảng"
                    },
                    new LoaiDuLieu()
                    {
                        Id = 4,
                        Ten = "Theo Lượt ra Cối (Sản lượng lần ra Cối)",
                    },
                    new LoaiDuLieu()
                    {
                        Id = 5,
                        Ten = "Sản Lượng Không Quét Thẻ"
                    },
                    new LoaiDuLieu()
                    {
                        Id = 6,
                        Ten = "Sản Lượng Không Quét Thẻ Và Nhập"
                    },
                    new LoaiDuLieu()
                    {
                        Id = 7,
                        Ten = "Bảng (Lùi Giờ)"
                    },
                    new LoaiDuLieu()
                    {
                        Id = 8,
                        Ten = "Phục Vụ Phân Cỡ"
                    }
                };
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }
        //[ObservableProperty] private ObservableRangeCollection<string> employeeCodes = new();
        public static LoaiDuLieuViewModel Instance => instance ??= new LoaiDuLieuViewModel();
    }
}
