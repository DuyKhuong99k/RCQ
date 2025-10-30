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

namespace ViewModels.Repos.HQ
{
    public partial class KhuVucKetChuyenViewModel : ObservableObject
    {
        private static KhuVucKetChuyenViewModel instance;
        public static KhuVucKetChuyenViewModel Instance => instance ??= new KhuVucKetChuyenViewModel();
        public List<Models.Repos.NotiModel.KC_KhuVucTrangThai> Gets(DateTime dateTime)
        {
            try
            {
                var KhuVucTrangThais = new List<Models.Repos.NotiModel.KC_KhuVucTrangThai>();
                var logKetChuyenDao = new Dao.Repos.HQ.LogKetChuyenBravo();
                var phieuCanFilletDao = new Dao.Repos.HQ.PhieuCanFillet();

                var logKVSauFillets = logKetChuyenDao.Gets(dateTime);
                var khuVucs = new Models.Repos.NotiModel.KC_KhuVuc().Gets();
                foreach (var item in khuVucs)
                {
                    var items = logKVSauFillets.Where(x => x.tab == item.Tab).ToList();
                    if (items != null && items.Any())
                    {
                        var xuongIds = items.Select(x => x.MaXuong).Distinct().ToList();
                        if (xuongIds.Count > 1)
                        {
                            KhuVucTrangThais.Add(
                                new Models.Repos.NotiModel.KC_KhuVucTrangThai()
                                {
                                    Kc_KhuVuc = item,
                                    MaXuong = "0"
                                });
                        }
                        else
                        {
                            KhuVucTrangThais.Add(
                                new Models.Repos.NotiModel.KC_KhuVucTrangThai()
                                {
                                    Kc_KhuVuc = item,
                                    MaXuong = xuongIds[0]
                                });
                        }
                    }
                }
                //var filletCount = phieuCanFilletDao.GetsCount(dateTime);
                //if(filletCount > 0)
                //{
                //    KhuVucTrangThais.Add(
                //        new KC_KhuVucTrangThai()
                //        {
                //            Kc_KhuVuc =
                //            new KC_KhuVuc()
                //                    {
                //                        KhuVucId = "09",
                //                        Name = "Fillet",
                //                        Tab = 9
                //                    },
                //            MaXuong = "0"
                //        });
                //}
                return KhuVucTrangThais.ToList();
            }
            catch (Exception exception)
            {
                return new List<Models.Repos.NotiModel.KC_KhuVucTrangThai>();
                //MessageBox.Show(exception.Message);
                //throw;
            }
        }


    }
}

