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
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models.Repos;
using Models.Repos.Models;
using Models.Repos.SoketModels;
using ViewModels.Repos.API;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BoTriNhomKiemSoChesController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public BoTriNhomKiemSoChesController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/BoTriNhomKiemSoChes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BoTriNhomKiemSoChe>>> Gets()
        {
            if (_context.BoTriNhomKiemSoChe == null)
            {
                return NotFound();
            }
            return await _context.BoTriNhomKiemSoChe.ToListAsync();
        }

        // GET: api/BoTriNhomKiemSoChes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BoTriNhomKiemSoChe>> Get(string id)
        {
            if (_context.BoTriNhomKiemSoChe == null)
            {
                return NotFound();
            }
            var boTriNhomKiemSoChe = await _context.BoTriNhomKiemSoChe.FindAsync(id);

            if (boTriNhomKiemSoChe == null)
            {
                return NotFound();
            }

            return boTriNhomKiemSoChe;
        }

        // PUT: api/BoTriNhomKiemSoChes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, BoTriNhomKiemSoChe boTriNhomKiemSoChe)
        {
            if (id != boTriNhomKiemSoChe.MaNhanVien)
            {
                return BadRequest();
            }

            _context.Entry(boTriNhomKiemSoChe).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BoTriNhomKiemSoCheExists(id))
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

        // POST: api/BoTriNhomKiemSoChes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BoTriNhomKiemSoChe>> Post(BoTriNhomKiemSoChe boTriNhomKiemSoChe)
        {
            if (_context.BoTriNhomKiemSoChe == null)
            {
                return Problem("Entity set 'dbPMScontext.BoTriNhomKiemSoChe'  is null.");
            }
            _context.BoTriNhomKiemSoChe.Add(boTriNhomKiemSoChe);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BoTriNhomKiemSoCheExists(boTriNhomKiemSoChe.MaNhanVien))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBoTriNhomKiemSoChe", new { id = boTriNhomKiemSoChe.MaNhanVien }, boTriNhomKiemSoChe);
        }

        // DELETE: api/BoTriNhomKiemSoChes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.BoTriNhomKiemSoChe == null)
            {
                return NotFound();
            }
            var boTriNhomKiemSoChe = await _context.BoTriNhomKiemSoChe.FindAsync(id);
            if (boTriNhomKiemSoChe == null)
            {
                return NotFound();
            }

            _context.BoTriNhomKiemSoChe.Remove(boTriNhomKiemSoChe);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BoTriNhomKiemSoCheExists(string id)
        {
            return (_context.BoTriNhomKiemSoChe?.Any(e => e.MaNhanVien == id)).GetValueOrDefault();
        }
        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> LoadBoTriNhomKiemSoChe(string dateTime, string xuongId)
        {
            if (_context.BoTriNhomSoChe == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmBoTriNhomKiemSoChe.LoadNhomKiemSoChe(date1, xuongId);
            return items;
        }
        [HttpGet("{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetNhomKiemSoChe(string xuongId)
        {
            if (_context.BoTriNhomKiemSoChe == null)
            {
                return NotFound();
            }
            var items = Vm.VmNhomKiemSoChe.Gets(xuongId);
            return items;
        }
        [HttpGet("{dateTime}/{xuongId}/{nhomKiemSoChe}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> NhomSoCheSelectionChanged(string dateTime, string xuongId, string nhomKiemSoChe)
        {
            if (_context.BoTriNhomSoChe == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmNhomKiemSoChe.NhomKiemSoCheSelectionChanged(date1, xuongId, nhomKiemSoChe);
            return items;
        }

        [HttpPost("{dateTime}/{xuongId}/{nhomKiemSoChe}")]
        [Authorize]
        public async Task<IActionResult> MoveTo(string dateTime, string xuongId, string nhomKiemSoChe, Tuple<string> dataT)
        {
            try
            {
                DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                if (_context.NhanVienDaiThanh == null)
                {
                    return NotFound();
                }
                Vm.VmBoTriNhomKiemSoChe.MoveTo(date1,xuongId,nhomKiemSoChe, dataT);
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
        [HttpPost("{dateTime}/{xuongId}/{nhomKiemSoChe}")]
        [Authorize]
        public async Task<IActionResult> Remove(string dateTime, string xuongId, string nhomKiemSoChe, Tuple<string> dataT)
        {
            try
            {
                DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                if (_context.NhanVienDaiThanh == null)
                {
                    return NotFound();
                }
                Vm.VmBoTriNhomKiemSoChe.Remove(date1,xuongId,nhomKiemSoChe, dataT);
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
        [HttpPost("{dateTime}/{xuongId}/{nhomKiemSoChe}")]
        [Authorize]
        public async Task<IActionResult> Add(string dateTime, string xuongId, string nhomKiemSoChe, Tuple<string> dataT)
        {
            try
            {
                DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                if (_context.NhanVienDaiThanh == null)
                {
                    return NotFound();
                }
                Vm.VmBoTriNhomKiemSoChe.Add(date1,xuongId,nhomKiemSoChe, dataT);
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
