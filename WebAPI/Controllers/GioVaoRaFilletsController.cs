using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
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
using Microsoft.AspNetCore.SignalR;
using Models.Repos.SoketModels;
using ViewModels.Repos.HQ;
using Microsoft.AspNetCore.JsonPatch.Internal;


namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GioVaoRaFilletsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public GioVaoRaFilletsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/GioVaoRaFillets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GioVaoRaFillet>>> Gets()
        {
            if (_context.GioVaoRaFillet == null)
            {
                return NotFound();
            }
            return await _context.GioVaoRaFillet.ToListAsync();
        }

        // GET: api/GioVaoRaFillets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GioVaoRaFillet>> GetGioVaoRaFillet(int id)
        {
            if (_context.GioVaoRaFillet == null)
            {
                return NotFound();
            }
            var gioVaoRaFillet = await _context.GioVaoRaFillet.FindAsync(id);

            if (gioVaoRaFillet == null)
            {
                return NotFound();
            }

            return gioVaoRaFillet;
        }

        // PUT: api/GioVaoRaFillets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGioVaoRaFillet(int id, GioVaoRaFillet gioVaoRaFillet)
        {
            if (id != gioVaoRaFillet.STT)
            {
                return BadRequest();
            }

            _context.Entry(gioVaoRaFillet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GioVaoRaFilletExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/GioVaoRaFillets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GioVaoRaFillet>> PostGioVaoRaFillet(GioVaoRaFillet gioVaoRaFillet)
        {
            if (_context.GioVaoRaFillet == null)
            {
                return Problem("Entity set 'dbPMScontext.GioVaoRaFillet'  is null.");
            }
            _context.GioVaoRaFillet.Add(gioVaoRaFillet);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (GioVaoRaFilletExists(gioVaoRaFillet.STT))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetGioVaoRaFillet", new { id = gioVaoRaFillet.STT }, gioVaoRaFillet);
        }

        // DELETE: api/GioVaoRaFillets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGioVaoRaFillet(int id)
        {
            if (_context.GioVaoRaFillet == null)
            {
                return NotFound();
            }
            var gioVaoRaFillet = await _context.GioVaoRaFillet.FindAsync(id);
            if (gioVaoRaFillet == null)
            {
                return NotFound();
            }

            _context.GioVaoRaFillet.Remove(gioVaoRaFillet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GioVaoRaFilletExists(int id)
        {
            return (_context.GioVaoRaFillet?.Any(e => e.STT == id)).GetValueOrDefault();
        }

        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> LoadGioRaVaoFillet(string dateTime, string xuongId)
        {
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmGioRaVaoFillet.Get(date1, xuongId);
            return items;
        }
        [HttpGet("{dateTime}/{xuongId}/{thanhPham}/{maNhanVien}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> NhanViensFindGhiNhanSelectedItemChanged(string dateTime, string xuongId, string thanhPham, string maNhanVien)
        {
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmNhanVien.NhanViensFindGhiNhanSelectedItemChanged(date1, xuongId, thanhPham, maNhanVien);
            return items;
        }
        [HttpPost("{dateTime}/{xuongId}/{thanhPham}/{gioVao}/{gioRa}")]
        [Authorize]
        public async Task<IActionResult> SetGioVaoRaMacDinh(string dateTime, string xuongId, string thanhPham, string gioVao, string gioRa, Tuple<string> dataT)
        {
            try
            {
                DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                TimeSpan timeIn = TimeSpan.Parse(gioVao);
                TimeSpan timeOut = TimeSpan.Parse(gioRa);

                Vm.VmGioRaVaoFillet.SetGioVaoRaMacDinh(date1, xuongId, timeIn, timeOut, thanhPham, dataT);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Thành công!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi.",
                    Errors = new List<string> { ex.Message }
                }); ;
            }

        }
    }
}
