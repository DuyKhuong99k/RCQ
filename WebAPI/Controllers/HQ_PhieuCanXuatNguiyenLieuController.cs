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
    public class HQ_PhieuCanXuatNguiyenLieuController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public HQ_PhieuCanXuatNguiyenLieuController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;

        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public IActionResult GetChiTietPhieuCaXuatNguyenLieus(string fromDate, string toDate,string xuongId)
        {
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            var items = Vm.VmHQ_PhieuCanXuatNguyenLieu.GetChiTietPhieuCaXuatNguyenLieus<object>(from, to,xuongId, _context.Database.GetConnectionString());
            return Ok(items);
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public IActionResult GetTongHopSanPhamPhieuCanXuatNguyenLieus(string fromDate, string toDate,string xuongId)
        {
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            var items = Vm.VmHQ_PhieuCanXuatNguyenLieu.GetTongHopSanPhamPhieuCanXuatNguyenLieus<object>(from, to, xuongId,_context.Database.GetConnectionString());
            return Ok(items);
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public IActionResult GetTonKhoSanPhamPhieuCanXuatNguyenLieus(string fromDate, string toDate,string xuongId)
        {
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            var items = Vm.VmHQ_PhieuCanXuatNguyenLieu.GetTonKhoSanPhamPhieuCanXuatNguyenLieus<object>(from, to, xuongId,_context.Database.GetConnectionString());
            return Ok(items);
        }

        [HttpGet("{dateTime}")]
        [Authorize]
        public IActionResult GetTyLeNguyenLieuHaoHut(string dateTime)
        {
            DateTime ngay = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
          
            var items = Vm.VmHQ_PhieuCanXuatNguyenLieu.GetTyLeNguyenLieuHaoHut<object>(ngay,_context.Database.GetConnectionString());
            return Ok(items);
        }


        [HttpGet("{maLo}")]
        [Authorize]
        public IActionResult GetKhoiLuongXuatLoTheoXuong(string maLo)
        {        
            var items = Vm.VmHQ_PhieuCanXuatNguyenLieu.GetKhoiLuongXuatLoTheoXuonget<object>(maLo, _context.Database.GetConnectionString());
            return Ok(items);
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetListLo()
        {        
            var items = Vm.VmHQ_PhieuCanXuatNguyenLieu.GetListLo<object>(_context.Database.GetConnectionString());
            return Ok(items);
        }
        
    }
}
