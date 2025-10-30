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
using Models.Repos.AppModel;
using System.Collections.ObjectModel;

namespace ViewModels.Repos.HQ
{
    public partial class SanPhamBravoViewModel : ObservableObject
    {
        private static SanPhamBravoViewModel instance;
        public static SanPhamBravoViewModel Instance => instance ??= new SanPhamBravoViewModel();

         [ObservableProperty] private ObservableRangeCollection<SanPhamBravo> sanPhamBravoes = new();
         private SanPhamBravoViewModel()
        {
            try
            {
                //SanPhamBravoes = new ObservableCollection<Models.Repos.AppModel.SanPhamBravo>(GetSanPhamBravoes());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // throw;
            }
        }

        public SanPhamBravo GetSanPhamKiemSoChe()
        {
            try
            {
                return new SanPhamBravo()
                {
                    Id = "SP-216",
                    Name = "Kiểm cá muối"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public BravoModelV1.Model.SanPhamBravo GetSanPhamKiem()
        {
            try
            {
                return new BravoModelV1.Model.SanPhamBravo()
                {
                    Id = "SP-035",
                    Name = "Kiểm định hình"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public BravoModelV1.Model.SanPhamBravo GetSanPhamPhucVu()
        {
            try
            {
                return new BravoModelV1.Model.SanPhamBravo()
                {
                    Id = "SP-102",
                    Name = "Phục vụ định hình"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public List<BravoModelV1.Model.SanPhamBravo> GetSanPhamBravoes()
        {
            try
            {
                //var items = new List<SanPhamBravo>()
                //{
                //    new SanPhamBravo()
                //    {
                //        Id = "SP-217",
                //        Name = "ĐH>",
                //        DaiThanhId = "A"
                //    },
                //    new SanPhamBravo()
                //    {
                //        Id = "SP-218",
                //        Name = "ĐH<",
                //        DaiThanhId = "B"
                //    },
                //    new SanPhamBravo()
                //    {
                //        Id = "SP-219",
                //        Name = "LN (4kg)",
                //        DaiThanhId = "C"
                //    },
                //    new SanPhamBravo()
                //    {
                //        Id = "SP-220",
                //        Name = "Dat Nho",
                //        DaiThanhId = "D"
                //    },
                //    new SanPhamBravo()
                //    {
                //        Id = "SP-221",
                //        Name = "XDa>",
                //        DaiThanhId = "E"
                //    },
                //    new SanPhamBravo()
                //    {
                //        Id = "SP-222",
                //        Name = "XDa<",
                //        DaiThanhId = "F"
                //    },
                //    new SanPhamBravo()
                //    {
                //        Id = "SP-223",
                //        Name = "CDBMBD>",
                //        DaiThanhId = "G"
                //    },
                //    new SanPhamBravo()
                //    {
                //        Id = "SP-224",
                //        Name = "CDBMBD<",
                //        DaiThanhId = "H"
                //    },
                //    new SanPhamBravo()
                //    {
                //        Id = "SP-225",
                //        Name = "HFood",
                //        DaiThanhId = "I"
                //    },
                //    new SanPhamBravo()
                //    {
                //        Id = "SP-226",
                //        Name = "Semi BMBDCD",
                //        DaiThanhId = "J"
                //    },
                //    new SanPhamBravo()
                //    {
                //        Id = "SP-227",
                //        Name = "Semi BCBMBDCD",
                //        DaiThanhId = "K"
                //    },
                //    new SanPhamBravo()
                //    {
                //        Id = "SP-228",
                //        Name = "CO",
                //        DaiThanhId = "L"
                //    },
                //    new SanPhamBravo()
                //    {
                //        Id = "SP-229",
                //        Name = "KXL>",
                //        DaiThanhId = "M"
                //    },
                //    new SanPhamBravo()
                //    {
                //        Id = "SP-230",
                //        Name = "KXL<",
                //        DaiThanhId = "N"
                //    },
                //    new SanPhamBravo()
                //    {
                //        Id = "SP-231",
                //        Name = "KXL 4Kg",
                //        DaiThanhId = "O"
                //    },
                //    new SanPhamBravo()
                //    {
                //        Id = "SP-232",
                //        Name = "CDa Xda",
                //        DaiThanhId = "P"
                //    },
                //    new SanPhamBravo()
                //    {
                //        Id = "SP-233",
                //        Name = "ĐHCCĐ>",
                //        DaiThanhId = "Q"
                //    },
                //    new SanPhamBravo()
                //    {
                //        Id = "SP-234",
                //        Name = "CDa De 2cm",
                //        DaiThanhId = "R"
                //    },
                //    new SanPhamBravo()
                //    {
                //        Id = "SP-235",
                //        Name = "Cao Frabel",
                //        DaiThanhId = "S"
                //    },
                //    new SanPhamBravo()
                //    {
                //        Id = "SP-236",
                //        Name = "CĐỎ",
                //        DaiThanhId = "T"
                //    },
                //    new SanPhamBravo()
                //    {
                //        Id = "SP-228",
                //        Name = "CO>",
                //        DaiThanhId = "001"
                //    },
                //    new SanPhamBravo()
                //    {
                //        Id = "SP-238",
                //        Name = "CO<",
                //        DaiThanhId = "002"
                //    },
                //    new SanPhamBravo()
                //    {
                //        Id = "SP-233",
                //        Name = "ĐHCCĐ>",
                //        DaiThanhId = "003"
                //    },
                //    new SanPhamBravo()
                //    {
                //        Id = "SP-237",
                //        Name = "ĐHCCĐ<",
                //        DaiThanhId = "004"
                //    },
                //    new SanPhamBravo()
                //    {
                //        Id = "SP-236",
                //        Name = "CĐỏ>",
                //        DaiThanhId = "005"
                //    },
                //    new SanPhamBravo()
                //    {
                //        Id = "SP-239",
                //        Name = "CĐỏ<",
                //        DaiThanhId = "006"
                //    },
                //    new SanPhamBravo()
                //    {
                //        Id = "SP-226",
                //        Name = "Semi BMBDCD>",
                //        DaiThanhId = "007"
                //    },
                //    new SanPhamBravo()
                //    {
                //        Id = "SP-240",
                //        Name = "Semi BMBDCD<",
                //        DaiThanhId = "008"
                //    },
                //    new SanPhamBravo()
                //    {
                //        Id = "SP-217",
                //        Name = "ĐH-TN",
                //        DaiThanhId ="009"
                //    },
                //    new SanPhamBravo()
                //    {
                //        Id = "SP-217",
                //        Name = "ĐH-TN01",
                //        DaiThanhId ="010"
                //    }
                //};
                //return items;
                var items = ViewModels.Repos.HQ.ThanhPhamDinhHinhViewModel.Instance.GetsSanPhamTinhLuong<BravoModelV1.Model.SanPhamBravo>();
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
