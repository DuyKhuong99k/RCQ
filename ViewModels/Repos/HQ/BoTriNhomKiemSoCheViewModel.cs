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
using System.Text.Json;
namespace ViewModels.Repos.HQ
{
    public partial class BoTriNhomKiemSoCheViewModel : ObservableObject
    {
        private static BoTriNhomKiemSoCheViewModel instance;
        public static BoTriNhomKiemSoCheViewModel Instance => instance ??= new BoTriNhomKiemSoCheViewModel();
        public int Delete(BoTriNhomKiemSoChe entities, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.BoTriNhomKiemSoChe(connStr);
                return dao.Delete(entities);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Insert(BoTriNhomKiemSoChe entities, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.BoTriNhomKiemSoChe(connStr);
                return dao.Insert(entities);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<BoTriNhomKiemSoChe> Gets(DateTime dateTime, string xuongId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.BoTriNhomKiemSoChe(connStr);
                return dao.Gets(dateTime, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<BoTriNhomKiemSoChe> GetWithNhoms(DateTime dateTime, string xuongId, string nhomId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.BoTriNhomKiemSoChe(connStr);
                return dao.Gets(dateTime, xuongId, nhomId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<BoTriNhomKiemSoChe> LoadNhomKiemSoChe(DateTime dateTime, string xuongId)
        {
            try
            {
                var nhomKiemsSoChe = new List<BoTriNhomKiemSoChe>(Gets(dateTime, xuongId));
                if (nhomKiemsSoChe == null || !nhomKiemsSoChe.Any())
                {
                    nhomKiemsSoChe = new List<BoTriNhomKiemSoChe>(Gets(new DateTime(2020, 03, 21), xuongId));
                    if (nhomKiemsSoChe != null && nhomKiemsSoChe.Any())
                        nhomKiemsSoChe.All(
                            x =>
                            {
                                x.Ngay = dateTime.Date;
                                return true;
                            });
                }
                return nhomKiemsSoChe.ToList();
            }
            catch (Exception exception)
            {
                return new List<BoTriNhomKiemSoChe>();
                //MessageBox.Show(exception.Message);
                //throw;
            }
        }

        public void MoveTo(DateTime dateTime, string xuongId, string nhomKiemSoChe, Tuple<string> dataT)
        {
            try
            {

                string listBoTriNhomKiemSoCheSelectedItems = dataT.Item1;
                string[] nhanVienKiemSoChes = listBoTriNhomKiemSoCheSelectedItems.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var items = new List<BoTriNhomKiemSoChe>();
                foreach (var nhanVienKiemSoCheSelectedItem in nhanVienKiemSoChes)
                {
                    // Làm sạch chuỗi JSON escape
                    string cleanedJson = nhanVienKiemSoCheSelectedItem.Replace("\\\"", "\"").Replace("\\\\", "\\").Trim('"');

                    // Deserialize JSON thành đối tượng NhanVienKiem
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var nhanVienKiemSoChe = JsonSerializer.Deserialize<BoTriNhomKiemSoChe>(cleanedJson, options);

                    // Thêm đối tượng vào danh sách items nếu nó không null
                    if (nhanVienKiemSoChe != null)
                    {
                        items.Add(nhanVienKiemSoChe);
                    }
                }
                foreach (var nhanVien in items)
                {
                    Delete(nhanVien);
                    var nv = NhanVienViewModel.Instance.NhanVienDaiThanhs
                        .SingleOrDefault(x => x.MaNhanVien == nhanVien.MaNhanVien);
                    if (nv != null)
                    {
                        var nhanVienKiem = new BoTriNhomKiemSoChe()
                        {
                            MaNhanVien = nhanVien.MaNhanVien,
                            MaNhomKiem = nhomKiemSoChe,
                            MaXuong = nhanVien.MaXuong,
                            Ngay = nhanVien.Ngay,
                            SoGio = nhanVien.SoGio,
                            TyLeHuong = nhanVien.TyLeHuong,
                            TyLeTru = nhanVien.TyLeTru
                        };
                        Insert(nhanVienKiem);
                    }
                }
            }
            catch (Exception exception)
            {
                //MessageBox.Show(exception.Message);
                //throw;
            }
        }

        public void Add(DateTime dateTime, string xuongId, string nhomKiemSoChe, Tuple<string> dataT)
        {
            try
            {

                string listNhanVienDaiThanhSelectedItems = dataT.Item1;
                string[] nhanVienDaiThanhSelectItems = listNhanVienDaiThanhSelectedItems.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var items = new List<NhanVienDaiThanh>();
                foreach (var nhanVien in nhanVienDaiThanhSelectItems)
                {
                    // Làm sạch chuỗi JSON escape
                    string cleanedJson = nhanVien.Replace("\\\"", "\"").Replace("\\\\", "\\").Trim('"');

                    // Deserialize JSON thành đối tượng NhanVienKiem
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var nhanVienDaiThanhSelectItem = JsonSerializer.Deserialize<NhanVienDaiThanh>(cleanedJson, options);

                    // Thêm đối tượng vào danh sách items nếu nó không null
                    if (nhanVienDaiThanhSelectItem != null)
                    {
                        items.Add((NhanVienDaiThanh)nhanVienDaiThanhSelectItem);
                    }
                    //items.Add((NhanVienDaiThanh)nhanVien);
                }


                foreach (var nhanVienDaiThanh in items)
                {
                    var nhanVienKiem = new BoTriNhomKiemSoChe()
                    {
                        MaNhanVien = nhanVienDaiThanh.MaNhanVien,
                        MaNhomKiem = nhomKiemSoChe,
                        MaXuong = xuongId,
                        Ngay = dateTime,
                        SoGio = 8,
                        TyLeHuong = 1,
                        TyLeTru = 0
                    };

                    var nhomKiemSoChes = LoadNhomKiemSoChe(dateTime, xuongId);
                    var item = nhomKiemSoChes.SingleOrDefault(
                        x => x.MaNhanVien == nhanVienKiem.MaNhanVien &&
                             x.MaNhomKiem == nhanVienKiem.MaNhomKiem);
                    if (item == null)
                    {
                        Insert(nhanVienKiem);
                        //var itemTo = NhanViensNhom.SingleOrDefault(
                        //    x => x.MaNhanVien == nhanVienDaiThanh.MaNhanVien);
                        //if (itemTo == null)
                        //{
                        //    NhanViensNhom.Add(nhanVienDaiThanh);
                        //}
                    }
                    else
                    {
                        //MessageBox.Show(
                        //    $@"Nhân viên {nhanVienDaiThanh.MaNhanVien}:{nhanVienDaiThanh.MaHoSo}:{nhanVienDaiThanh.Name} đã tồn tại trong tổ {item.MaNhomKiem}");
                    }
                }

            }
            catch (Exception exception)
            {
                //MessageBox.Show(exception.Message);
                //throw;
            }
        }
        public void Remove(DateTime dateTime, string xuongId, string nhomKiemSoChe, Tuple<string> dataT)
        {
            try
            {
                string listNhanVienDaiThanhSelectedItems = dataT.Item1;
                string[] nhanVienDaiThanhSelectItems = listNhanVienDaiThanhSelectedItems.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var items = new List<NhanVienDaiThanh>();
                foreach (var nhanVien in nhanVienDaiThanhSelectItems)
                {
                    // Làm sạch chuỗi JSON escape
                    string cleanedJson = nhanVien.Replace("\\\"", "\"").Replace("\\\\", "\\").Trim('"');

                    // Deserialize JSON thành đối tượng NhanVienKiem
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var nhanVienDaiThanhSelectItem = JsonSerializer.Deserialize<NhanVienDaiThanh>(cleanedJson, options);

                    // Thêm đối tượng vào danh sách items nếu nó không null
                    if (nhanVienDaiThanhSelectItem != null)
                    {
                        items.Add((NhanVienDaiThanh)nhanVienDaiThanhSelectItem);
                    }
                    //items.Add((NhanVienDaiThanh)nhanVien);
                }

                foreach (var nhanVienDaiThanh in items)
                {
                    var nhomKiemSoChes = LoadNhomKiemSoChe(dateTime, xuongId);
                    //NhanViensNhom.Remove(nhanVienDaiThanh);
                    var item = nhomKiemSoChes.SingleOrDefault(
                        x => x.MaNhanVien == nhanVienDaiThanh.MaNhanVien &&
                             x.MaNhomKiem == nhomKiemSoChe);
                    if (item != null)
                    {
                        Delete(item);
                    }
                }
            }
            catch (Exception exception)
            {
                //MessageBox.Show(exception.Message);
                //throw;
            }
        }
    }
}
