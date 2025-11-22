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
    public class HQ_PhieuCanNhapNguyenLieuController : ControllerBase
    { 
        private readonly dbPMScontext _context;

        public HQ_PhieuCanNhapNguyenLieuController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;

        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public IActionResult GetChiTietPhieuCanNhapNguyenLieus(string fromDate, string toDate,string xuongId)
        {
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            var items = Vm.VmHQ_PhieuCanNhapNguyenLieu.GetChiTietPhieuCanNhapNguyenLieus<object>(from, to,xuongId, _context.Database.GetConnectionString());
            return Ok(items);
        }
        [HttpGet("{fromDate}/{toDate}/{xuongId}")]
        [Authorize]
        public IActionResult GetTongHopSanPhamPhieuCanNguyenLieus(string fromDate, string toDate,string xuongId)
        {
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            var items = Vm.VmHQ_PhieuCanNhapNguyenLieu.GetTongHopSanPhamPhieuCanNguyenLieus<object>(from, to, xuongId,_context.Database.GetConnectionString());
            return Ok(items);
        }
        
        
    }
}
