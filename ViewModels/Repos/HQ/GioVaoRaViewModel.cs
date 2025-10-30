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

namespace ViewModels.Repos.HQ
{
    public partial class GioVaoRaViewModel
    {
        private static GioVaoRaViewModel instance;
        public static GioVaoRaViewModel Instance => instance ??= new GioVaoRaViewModel();
        public List<TimeSpan> GetListTime(DateTime dateTime, IEnumerable<string> ids, string congViecId, string xuongId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.GioVaoRaFillet(connStr);
                return dao.GetListTime(dateTime, ids, congViecId, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<TimeSpan> GetListTime(
            DateTime dateTime,
            string xuongId,
            string congViecId,
            IEnumerable<string> nhanVienIds, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.GioVaoRaTinhLuongXepKhuon(connStr);
                return dao.GetListTime(dateTime, xuongId, congViecId, nhanVienIds);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Models.Repos.Models.GioVaoRaFillet> Get(
            DateTime dateTime,
            TimeSpan fromTime,
            TimeSpan toTime,
            IEnumerable<string> ids,
            string congViecId,
            string xuongId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.GioVaoRaFillet(connStr);
                return dao.GetGioVaoRaFillet(dateTime, fromTime, toTime, ids, congViecId, xuongId);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        public List<Models.Repos.Models.GioVaoRaFillet> Gets(
            DateTime dateTime,
            string congViecId,
            IEnumerable<string> nhanVienIds,
            string xuongId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.GioVaoRaFillet(connStr);
                return dao.Gets(dateTime, congViecId, nhanVienIds, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Models.Repos.Models.GioVaoRaTinhLuongXepKhuon> Gets(
            DateTime dateTime,
            string xuongId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.GioVaoRaTinhLuongXepKhuon();
                return dao.Gets(dateTime, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Models.Repos.Models.GioVaoRaTinhLuongXepKhuon> Gets(
            DateTime dateTime,
            TimeSpan fromTime,
            TimeSpan toTime,
            string xuongId,
            IEnumerable<string> ids,
            string congViecId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.GioVaoRaTinhLuongXepKhuon(connStr);
                return dao.Gets(dateTime, fromTime, toTime, xuongId, ids, congViecId);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        public List<Models.Repos.Models.GioVaoRaTinhLuongXepKhuon> Gets(
            DateTime dateTime,
            TimeSpan fromTime,
            TimeSpan toTime,
            string xuongId,
            IEnumerable<string> ids, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.GioVaoRaTinhLuongXepKhuon(connStr);
                return dao.Gets(dateTime, fromTime, toTime, xuongId, ids);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        public List<Models.Repos.Models.GioVaoRaFillet> Get(DateTime dateTime, string xuongId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.GioVaoRaFillet(connStr);
                return dao.GetGioVaoRaFillet(dateTime, xuongId);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        public List<Models.Repos.Models.GioVaoRaFillet> GetGioVaoRa(
            string nhanVienId,
            DateTime dateTime,
            string congViecId,
            string xuongId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.GioVaoRaFillet(connStr);
                return dao.GetGioVaoRaFillet(nhanVienId, dateTime, congViecId, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Delete(DateTime dateTime, string nhanVienId, string congViecId, string xuongId, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.GioVaoRaFillet(connStr);
                return dao.Delete(dateTime, nhanVienId, congViecId, xuongId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Insert(GioVaoRaFillet gioVaoRaFillet, string? connStr = null)
        {
            try
            {
                var dao = new Dao.Repos.HQ.GioVaoRaFillet(connStr);
                return dao.Insert(gioVaoRaFillet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void SetGioVaoRaMacDinh(DateTime dateTime, string xuongId, TimeSpan gioVao, TimeSpan gioRa, string thanhPham, Tuple<string> dataT)
        {
            try
            {
                string listPhanBoNhanVienPhuFilletSelectedItems = dataT.Item1;
                string[] phanBoNhanVienPhuFillets = listPhanBoNhanVienPhuFilletSelectedItems.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var items = new List<BravoModelV1.Model.PhanBoNhanVienPhu>();
                foreach (var phanBoNhanVienPhuFilletSelectedItem in phanBoNhanVienPhuFillets)
                {
                    // Làm sạch chuỗi JSON escape
                    string cleanedJson = phanBoNhanVienPhuFilletSelectedItem.Replace("\\\"", "\"").Replace("\\\\", "\\").Trim('"');

                    // Deserialize JSON thành đối tượng NhanVienKiem
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var phanBoNhanVienPhuFillet = JsonSerializer.Deserialize<BravoModelV1.Model.PhanBoNhanVienPhu>(cleanedJson, options);

                    // Thêm đối tượng vào danh sách items nếu nó không null
                    if (phanBoNhanVienPhuFillet != null)
                    {
                        items.Add((BravoModelV1.Model.PhanBoNhanVienPhu)phanBoNhanVienPhuFillet);
                    }
                }


                foreach (var item in items)
                {
                    var phanBo = item as BravoModelV1.Model.PhanBoNhanVienPhu;
                    var delRows = Delete(
                        dateTime,
                        phanBo.MaNhanVien,
                        phanBo.MaCongViec,
                        xuongId);
                    var insRows = Insert(
                        new GioVaoRaFillet()
                        {
                            STT = 1,
                            MaNhanVien = phanBo.MaNhanVien,
                            GioRa = gioRa,
                            GioVao = gioVao,
                            MaCongViec = phanBo.MaCongViec,
                            Ngay = dateTime,
                            MaXuong = xuongId
                        });
                }

               // return NhanVienViewModel.Instance.NhanViensFindGhiNhanSelectedItemChanged(dateTime, xuongId, thanhPham, items);
            }
            catch (Exception exception)
            {
                //MessageBox.Show(exception.Message);
                //throw;
                //return new List<GioVaoRaFillet>();
            }
        }
        public List<Models.Repos.Models.GioVaoRaTinhLuongXepKhuon> GetsTheoCa(
            DateTime dateTime,
            string xuongId,
            string congViecId,
            string caId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.GioVaoRaTinhLuongXepKhuon();
                return dao.GetsTheoCa(dateTime, xuongId, congViecId, caId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<GioVaoRaTinhLuongXepKhuon> GetsTheoCa(
            DateTime dateTime,
            string xuongId,
            string congViecId)
        {
            try
            {
                var dao = new Dao.Repos.HQ.GioVaoRaTinhLuongXepKhuon();
                return dao.GetsTheoCa(dateTime, xuongId, congViecId);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
