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
    public class PhieuCanKeToansController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PhieuCanKeToansController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        #region Báo Cáo
        [HttpGet("{fromDate}/{toDate}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanKeToansPV(string fromDate, string toDate)
        {
            
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanKeToan.GetPhieuCanKeToansPV(date1, date2);
            return Ok(items);
        }
        [HttpGet("{fromDate}/{toDate}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanKeToansNhanVienPV(string fromDate, string toDate)
        {
            
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanKeToan.GetPhieuCanKeToansNhanVienPV(date1, date2);
            return Ok(items);
        }
        [HttpGet("{fromDate}/{toDate}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopSanPhamPhieuCanDinhHinhsPV(string fromDate, string toDate)
        {
            
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanKeToan.GetTongHopSanPhamPhieuCanDinhHinhsPV(date1, date2);
            return Ok(items);
        }
        [HttpGet("{fromDate}/{toDate}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanFilletsPV(string fromDate, string toDate)
        {
            
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanKeToan.GetPhieuCanFilletsPV(date1, date2);
            return Ok(items);
        }
        [HttpGet("{fromDate}/{toDate}/{khuVucId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetTongHopSanPhamPhieuCanKiemDinhhinhsPV(string fromDate, string toDate, int khuVucId)
        {
            
            DateTime date1 = DateTime.ParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanKeToan.GetTongHopSanPhamPhieuCanKiemDinhhinhsPV(date1, date2, khuVucId);
            return Ok(items);
        }
        #endregion
    }
}
