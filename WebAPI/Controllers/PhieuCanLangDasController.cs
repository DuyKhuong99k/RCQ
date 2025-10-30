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
    public class PhieuCanLangDasController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PhieuCanLangDasController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        [HttpGet("{fromDate}/{toDate}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanChiTiets(string fromDate, string toDate)
        {
            if (_context.PhieuCanLangDa == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanLangDa.GetChiTiets<object>(from, to);
            return items;
        }
        [HttpGet("{fromDate}/{toDate}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetPhieuCanTongHops(string fromDate, string toDate)
        {
            if (_context.PhieuCanLangDa == null)
            {
                return NotFound();
            }
            // DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime from = DateTime.ParseExact(fromDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmPhieuCanLangDa.GetTongHops<object>(from, to);
            return items;
        }

    }
}
