using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models.Repos;
using Models.Repos.Models;
using ViewModels.Repos.API;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BaoCaoNhanhsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public BaoCaoNhanhsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;


        [HttpGet("{fromDate}/{toDate}/{keyWord}/{typeSearch}/{khauValues}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetsTongHopThanhPhamByNhanViens(string fromDate, string toDate, string keyWord, int typeSearch, string khauValues, string xuongId)
        {
            IEnumerable<object> items = null;
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            switch (khauValues)
            {
                case "PhieuCanPhuPhamv2":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanPhuPhamv2.GetTongHopThanhPhamByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanPhuPhamv2.GetTongHopThanhPhamTheoByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanPhuPhamv2.GetTongHopThanhPhamByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanBTPFilletv2":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanBTPFilletv2.GetTongHopThanhPhamsByMaNhanVien<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanBTPFilletv2.GetTongHopThanhPhamsByMaHoSo<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanBTPFilletv2.GetTongHopThanhPhamsByMaThe<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanLangDa":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanLangDa.GetTongHopThanhPhamByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanLangDa.GetTongHopThanhPhamByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanLangDa.GetTongHopThanhPhamByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanTPFillet":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanTPFillet.GetTongHopThanhPhamByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanTPFillet.GetTongHopThanhPhamByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanTPFillet.GetPhieuCanTonghopThanhPhamByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanTPFilletv2":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanTPFilletv2.GetTongHopThanhPhamByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanTPFilletv2.GetTongHopThanhPhamByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanTPFilletv2.GetTongHopThanhPhamByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanSoCheDinhHinh":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanSoCheDinhHinh.GetTongHopThanhPhamsByMaNhanVien<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanSoCheDinhHinh.GetTongHopThanhPhamsByMaHoSo<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanSoCheDinhHinh.GetTongHopThanhPhamsByMaThe<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanBTPDinhHinh":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanBTPDinhHinh.GetTongHopThanhPhamByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanBTPDinhHinh.GetTongHopThanhPhamByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanBTPDinhHinh.GetTongHopThanhPhamByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanTPDinhHinh":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanTPDinhHinh.GetTongHopLoaiThanhPhamByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanTPDinhHinh.GetTongHopLoaiThanhPhamByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanTPDinhHinh.GetTongHopLoaiThanhPhamByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanChinhXepKhuon":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanChinhXepKhuon.GetTongHopThanhPhamByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanChinhXepKhuon.GetTongHopThanhPhamByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanChinhXepKhuon.GetTongHopThanhPhamByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanPhuXepKhuon":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanPhuXepKhuon.GetTongHopThanhPhamByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanPhuXepKhuon.GetTongHopThanhPhamByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanPhuXepKhuon.GetTongHopThanhPhamByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanXepKhuonBlock":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanXepKhuonBlock.GetTongHopThanhPhamByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanXepKhuonBlock.GetTongHopThanhPhamByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanXepKhuonBlock.GetTongHopThanhPhamByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanXepKhuonKHC":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanXepKhuonKHC.GetsTongHopThanhPhamByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanXepKhuonKHC.GetsTongHopThanhPhamByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanXepKhuonKHC.GetsTongHopThanhPhamByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanTaiChe":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanTaiChe.GetTongHopThanhPhamByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanTaiChe.GetTongHopThanhPhamByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanTaiChe.GetTongHopThanhPhamByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "BT_PhieuCan":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmBT_PhieuCan.GetTongHopThanhPhamByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmBT_PhieuCan.GetTongHopThanhPhamByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmBT_PhieuCan.GetTongHopThanhPhamByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;

                default:
                    return BadRequest("Invalid khauValues value.");
            }


            return items == null ? NotFound() : Ok(items);
        }
        [HttpGet("{fromDate}/{toDate}/{keyWord}/{typeSearch}/{khauValues}/{xuongId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetsTongHopThanhPhamByNhanVienPublicAccess(string fromDate, string toDate, string keyWord, int typeSearch, string khauValues, string xuongId)
        {
            IEnumerable<object> items = null;
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            switch (khauValues)
            {
                case "PhieuCanPhuPhamv2":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanPhuPhamv2.GetTongHopThanhPhamByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanPhuPhamv2.GetTongHopThanhPhamTheoByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanPhuPhamv2.GetTongHopThanhPhamByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanBTPFilletv2":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanBTPFilletv2.GetTongHopThanhPhamsByMaNhanVien<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanBTPFilletv2.GetTongHopThanhPhamsByMaHoSo<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanBTPFilletv2.GetTongHopThanhPhamsByMaThe<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanLangDa":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanLangDa.GetTongHopThanhPhamByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanLangDa.GetTongHopThanhPhamByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanLangDa.GetTongHopThanhPhamByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanTPFillet":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanTPFillet.GetTongHopThanhPhamByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanTPFillet.GetTongHopThanhPhamByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanTPFillet.GetPhieuCanTonghopThanhPhamByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanTPFilletv2":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanTPFilletv2.GetTongHopThanhPhamByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanTPFilletv2.GetTongHopThanhPhamByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanTPFilletv2.GetTongHopThanhPhamByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanSoCheDinhHinh":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanSoCheDinhHinh.GetTongHopThanhPhamsByMaNhanVien<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanSoCheDinhHinh.GetTongHopThanhPhamsByMaHoSo<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanSoCheDinhHinh.GetTongHopThanhPhamsByMaThe<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanBTPDinhHinh":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanBTPDinhHinh.GetTongHopThanhPhamByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanBTPDinhHinh.GetTongHopThanhPhamByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanBTPDinhHinh.GetTongHopThanhPhamByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanTPDinhHinh":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanTPDinhHinh.GetTongHopLoaiThanhPhamByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanTPDinhHinh.GetTongHopLoaiThanhPhamByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanTPDinhHinh.GetTongHopLoaiThanhPhamByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanChinhXepKhuon":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanChinhXepKhuon.GetTongHopThanhPhamByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanChinhXepKhuon.GetTongHopThanhPhamByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanChinhXepKhuon.GetTongHopThanhPhamByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanPhuXepKhuon":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanPhuXepKhuon.GetTongHopThanhPhamByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanPhuXepKhuon.GetTongHopThanhPhamByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanPhuXepKhuon.GetTongHopThanhPhamByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanXepKhuonBlock":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanXepKhuonBlock.GetTongHopThanhPhamByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanXepKhuonBlock.GetTongHopThanhPhamByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanXepKhuonBlock.GetTongHopThanhPhamByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanXepKhuonKHC":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanXepKhuonKHC.GetsTongHopThanhPhamByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanXepKhuonKHC.GetsTongHopThanhPhamByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanXepKhuonKHC.GetsTongHopThanhPhamByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanTaiChe":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanTaiChe.GetTongHopThanhPhamByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanTaiChe.GetTongHopThanhPhamByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanTaiChe.GetTongHopThanhPhamByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "BT_PhieuCan":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmBT_PhieuCan.GetTongHopThanhPhamByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmBT_PhieuCan.GetTongHopThanhPhamByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmBT_PhieuCan.GetTongHopThanhPhamByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;

                default:
                    return BadRequest("Invalid khauValues value.");
            }


            return items == null ? NotFound() : Ok(items);
        }

        [HttpGet("{fromDate}/{toDate}/{keyWord}/{typeSearch}/{khauValues}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetsChiTietByNhanViens(string fromDate, string toDate, string keyWord, int typeSearch, string khauValues, string xuongId)
        {
            IEnumerable<object> items = null;
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            switch (khauValues)
            {
                case "PhieuCanPhuPhamv2":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanPhuPhamv2.GetChiTietByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanPhuPhamv2.GetChiTietByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanPhuPhamv2.GetChiTietByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanBTPFilletv2":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanBTPFilletv2.GetChiTietByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanBTPFilletv2.GetChiTietByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanBTPFilletv2.GetChiTietByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanLangDa":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanLangDa.GetChiTietByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanLangDa.GetChiTietByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanLangDa.GetChiTietByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanTPFillet":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanTPFillet.GetPhieuCanChiTietByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanTPFillet.GetPhieuCanChiTietByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanTPFillet.GetPhieuCanChiTietByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanTPFilletv2":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanTPFilletv2.GetChiTiets2HNByMaNhanVien<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanTPFilletv2.GetChiTiets2HNByMaHoSo<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanTPFilletv2.GetChiTiets2HNByMeThe<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanSoCheDinhHinh":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanSoCheDinhHinh.GetPhieuCanSoCheDinhHinhByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanSoCheDinhHinh.GetPhieuCanSoCheDinhHinhByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanSoCheDinhHinh.GetPhieuCanSoCheDinhHinhByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanBTPDinhHinh":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanBTPDinhHinh.GetChiTietByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanBTPDinhHinh.GetChiTietByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanBTPDinhHinh.GetChiTietByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanTPDinhHinh":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanTPDinhHinh.GetChiTietByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanTPDinhHinh.GetChiTietByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanTPDinhHinh.GetChiTietByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanChinhXepKhuon":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanChinhXepKhuon.GetPhieuCanChinhXepKhuonsByMaNhanVien<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanChinhXepKhuon.GetPhieuCanChinhXepKhuonsByMaHoSo<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanChinhXepKhuon.GetPhieuCanChinhXepKhuonsByMaThe<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanPhuXepKhuon":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanPhuXepKhuon.GetPhieuCanPhuXepKhuonByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanPhuXepKhuon.GetPhieuCanPhuXepKhuonByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanPhuXepKhuon.GetPhieuCanPhuXepKhuonByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanXepKhuonBlock":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanXepKhuonBlock.GetPhieuCanBlockByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanXepKhuonBlock.GetPhieuCanBlockByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanXepKhuonBlock.GetPhieuCanBlockByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanXepKhuonKHC":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanXepKhuonKHC.GetPhieuCanXepKhuonKHCByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanXepKhuonKHC.GetPhieuCanXepKhuonKHCMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanXepKhuonKHC.GetPhieuCanXepKhuonKHCByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "PhieuCanTaiChe":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmPhieuCanTaiChe.GetPhieuCanTaiCheByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmPhieuCanTaiChe.GetPhieuCanTaiCheByMaHoSo<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmPhieuCanTaiChe.GetPhieuCanTaiCheByMaThe<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;
                case "BT_PhieuCan":
                    switch (typeSearch)
                    {
                        case 0:
                            items = Vm.VmBT_PhieuCan.GetChiTietByMaNhanViens<object>(from, to, keyWord, xuongId);
                            break;
                        case 1:
                            items = Vm.VmBT_PhieuCan.GetChiTietByMaHoSos<object>(from, to, keyWord, xuongId);
                            break;
                        case 2:
                            items = Vm.VmBT_PhieuCan.GetChiTietByMaThes<object>(from, to, keyWord, xuongId);
                            break;
                        default:
                            return BadRequest("Invalid typeSearch value.");
                    }
                    break;

                default:
                    return BadRequest("Invalid khauValues value.");
            }


            return items == null ? NotFound() : Ok(items);
        }
    }
}
