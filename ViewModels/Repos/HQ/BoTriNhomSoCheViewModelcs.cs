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
    public partial class BoTriNhomSoCheViewModel : ObservableObject
    {
        private static BoTriNhomSoCheViewModel instance;
        public static BoTriNhomSoCheViewModel Instance => instance ??= new BoTriNhomSoCheViewModel();
        public List<BoTriNhomSoChe> Gets(DateTime dateTime, string xuongId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.BoTriNhomSoChe(connStr);
                return dao.Gets(dateTime, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<BoTriNhomSoChe> GetWithNhomSoChes(DateTime dateTime, string xuongId, string nhomId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.BoTriNhomSoChe(connStr);
                return dao.Gets(dateTime, xuongId, nhomId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Inserts(List<BoTriNhomSoChe> entities, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.BoTriNhomSoChe(connStr);
                return dao.Insert(entities);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Insert(BoTriNhomSoChe entities, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.BoTriNhomSoChe(connStr);
                return dao.Insert(entities);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Updates(List<BoTriNhomSoChe> entities, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.BoTriNhomSoChe(connStr);
                return dao.Update(entities);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Deletes(List<BoTriNhomSoChe> entities, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.BoTriNhomSoChe(connStr);
                return dao.Delete(entities);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Delete(BoTriNhomSoChe entities, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.BoTriNhomSoChe(connStr);
                return dao.Delete(entities);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<BoTriNhomSoChe> LoadBoTriNhomSoChe(DateTime dateTime, string xuongId)

        {
            try
            {
                var boTriNhomSoChes = new List<BoTriNhomSoChe>(Gets(dateTime, xuongId));
                if (boTriNhomSoChes == null || !boTriNhomSoChes.Any())
                {
                    boTriNhomSoChes = new List<BoTriNhomSoChe>(Gets(new DateTime(2020, 03, 21), xuongId));
                    if (boTriNhomSoChes != null && boTriNhomSoChes.Any())
                        boTriNhomSoChes.All(
                            x =>
                            {
                                x.Ngay = dateTime.Date;
                                return true;
                            });
                }
                return boTriNhomSoChes;
            }
            catch (Exception exception)
            {
                return new List<BoTriNhomSoChe>();
                //MessageBox.Show(exception.Message);
                //throw;
            }
        }

        public void SaveCommand(DateTime dateTime, string xuongId)
        {
            try
            {
                var tokiemsOgrin = Gets(dateTime, xuongId);
                var tokiemsDelete = new List<BoTriNhomSoChe>();
                var toKiemsInsert = new List<BoTriNhomSoChe>();
                var toKiemsUpdate = new List<BoTriNhomSoChe>();
                var boTriNhomSoChes = new List<BoTriNhomSoChe>(Gets(dateTime, xuongId));
                if (boTriNhomSoChes == null || !boTriNhomSoChes.Any())
                {
                    boTriNhomSoChes = new List<BoTriNhomSoChe>(Gets(new DateTime(2020, 03, 21), xuongId));
                    if (boTriNhomSoChes != null && boTriNhomSoChes.Any())
                        boTriNhomSoChes.All(
                            x =>
                            {
                                x.Ngay = dateTime.Date;
                                return true;
                            });
                }
                var nhomKiems = boTriNhomSoChes.ToList();
                foreach (var danhSachToKiem in tokiemsOgrin)
                {
                    var item = nhomKiems.SingleOrDefault(
                        x => x.MaNhomSoChe == danhSachToKiem.MaNhomSoChe &&
                             x.MaNhanVien == danhSachToKiem.MaNhanVien);

                    if (item != null)
                    {
                        nhomKiems.Remove(item);
                        toKiemsUpdate.Add(
                            new BoTriNhomSoChe()
                            {
                                MaNhomSoChe = item.MaNhomSoChe,
                                MaXuong = xuongId,
                                Ngay = dateTime,
                                MaNhanVien = item.MaNhanVien,
                                SoGio = item.SoGio,
                                TyLeHuong = item.TyLeHuong,
                                TyLeTru = item.TyLeTru
                            });
                    }
                    else
                    {
                        tokiemsDelete.Add(danhSachToKiem);
                    }
                }

                foreach (var nhanViensKiem in nhomKiems)
                {
                    var item = new BoTriNhomSoChe()
                    {
                        MaNhomSoChe = nhanViensKiem.MaNhomSoChe,
                        MaXuong = xuongId,
                        Ngay = dateTime,
                        MaNhanVien = nhanViensKiem.MaNhanVien,
                        SoGio = nhanViensKiem.SoGio,
                        TyLeHuong = nhanViensKiem.TyLeHuong,
                        TyLeTru = nhanViensKiem.TyLeTru
                    };
                    toKiemsInsert.Add(item);
                }

                //PhieuCanViewModel.Ins.IsBusy = true;
                //await Task.Delay(1000);
                var del = Deletes(tokiemsDelete);
                var ins = Inserts(toKiemsInsert);
                var uda = Updates(toKiemsUpdate);

                //await Task.Delay(1000);
                //PhieuCanViewModel.Ins.IsBusy = false;
                //MessageBox.Show($@"Đã thực hiện xong");
            }
            catch (Exception exception)
            {
                //MessageBox.Show(exception.Message);
                //throw;
            }
        }
        public void MoveTo(DateTime dateTime, string xuongId, string nhomSoChe, Tuple<string> dataT)
        {
            try
            {
                string listBoTriNhomSoCheSelectedItems = dataT.Item1;
                string[] nhanVienKiems = listBoTriNhomSoCheSelectedItems.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var items = new List<BoTriNhomSoChe>();
                foreach (var nhanVienKiemSelectedItem in nhanVienKiems)
                {
                    // Làm sạch chuỗi JSON escape
                    string cleanedJson = nhanVienKiemSelectedItem.Replace("\\\"", "\"").Replace("\\\\", "\\").Trim('"');

                    // Deserialize JSON thành đối tượng NhanVienKiem
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var nhanVienKiem = JsonSerializer.Deserialize<BoTriNhomSoChe>(cleanedJson, options);

                    // Thêm đối tượng vào danh sách items nếu nó không null
                    if (nhanVienKiem != null)
                    {
                        items.Add(nhanVienKiem);
                    }
                }
                var boTriNhomSoChes = new List<BoTriNhomSoChe>(Gets(dateTime, xuongId));
                if (boTriNhomSoChes == null || !boTriNhomSoChes.Any())
                {
                    boTriNhomSoChes = new List<BoTriNhomSoChe>(Gets(new DateTime(2020, 03, 21), xuongId));
                    if (boTriNhomSoChes != null && boTriNhomSoChes.Any())
                        boTriNhomSoChes.All(
                            x =>
                            {
                                x.Ngay = dateTime.Date;
                                return true;
                            });
                }

                foreach (var nhanVien in items)
                {
                    Delete(nhanVien);
                    var nv = NhanVienViewModel.Instance.NhanVienDaiThanhs
                            .SingleOrDefault(x => x.MaNhanVien == nhanVien.MaNhanVien);
                    if (nv != null)
                    {
                        if (nhanVien.MaNhomSoChe != nhomSoChe)
                        {
                            var nhanVienKiem = new BoTriNhomSoChe()
                            {
                                MaNhanVien = nhanVien.MaNhanVien,
                                MaNhomSoChe = nhomSoChe,
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // throw;
                //VmMessage.SetExceptionCommand.Execute(ex);
            }
        }

        public void Add(DateTime dateTime, string xuongId, string nhomSoChe, Tuple<string> dataT)
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
                    var nhanVienKiem = new BoTriNhomSoChe()
                    {
                        MaNhanVien = nhanVienDaiThanh.MaNhanVien,
                        MaNhomSoChe = nhomSoChe,
                        MaXuong = xuongId,
                        Ngay = dateTime,
                        SoGio = 8,
                        TyLeHuong = 1,
                        TyLeTru = 0
                    };

                    var boTriNhomSoChe = LoadBoTriNhomSoChe(dateTime, xuongId);
                    var item = boTriNhomSoChe.SingleOrDefault(
                        x => x.MaNhanVien == nhanVienKiem.MaNhanVien &&
                             x.MaNhomSoChe == nhanVienKiem.MaNhomSoChe);
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
                        //    $@"Nhân viên {nhanVienDaiThanh.MaNhanVien}:{nhanVienDaiThanh.MaHoSo}:{nhanVienDaiThanh.Name} đã tồn tại trong tổ {item.MaNhomSoChe}");
                    }
                }

            }
            catch (Exception exception)
            {
                //MessageBox.Show(exception.Message);
                //throw;
            }
        }

        public void Remove(DateTime dateTime, string xuongId, string nhomSoChe, Tuple<string> dataT)
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
                    var boTriNhomSoChe = LoadBoTriNhomSoChe(dateTime, xuongId);
                    var item = boTriNhomSoChe.SingleOrDefault(
                        x => x.MaNhanVien == nhanVienDaiThanh.MaNhanVien &&
                             x.MaNhomSoChe == nhomSoChe);
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
