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
    public class BoTriNhomSoChesController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public BoTriNhomSoChesController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;

        // GET: api/BoTriNhomSoChes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BoTriNhomSoChe>>> GetBoTriNhomSoChe()
        {
            if (_context.BoTriNhomSoChe == null)
            {
                return NotFound();
            }
            return await _context.BoTriNhomSoChe.ToListAsync();
        }

        // GET: api/BoTriNhomSoChes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BoTriNhomSoChe>> GetBoTriNhomSoChe(string id)
        {
            if (_context.BoTriNhomSoChe == null)
            {
                return NotFound();
            }
            var boTriNhomSoChe = await _context.BoTriNhomSoChe.FindAsync(id);

            if (boTriNhomSoChe == null)
            {
                return NotFound();
            }

            return boTriNhomSoChe;
        }

        // PUT: api/BoTriNhomSoChes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBoTriNhomSoChe(string id, BoTriNhomSoChe boTriNhomSoChe)
        {
            if (id != boTriNhomSoChe.MaNhanVien)
            {
                return BadRequest();
            }

            _context.Entry(boTriNhomSoChe).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BoTriNhomSoCheExists(id))
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

        // POST: api/BoTriNhomSoChes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BoTriNhomSoChe>> PostBoTriNhomSoChe(BoTriNhomSoChe boTriNhomSoChe)
        {
            if (_context.BoTriNhomSoChe == null)
            {
                return Problem("Entity set 'dbPMScontext.BoTriNhomSoChe'  is null.");
            }
            _context.BoTriNhomSoChe.Add(boTriNhomSoChe);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BoTriNhomSoCheExists(boTriNhomSoChe.MaNhanVien))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBoTriNhomSoChe", new { id = boTriNhomSoChe.MaNhanVien }, boTriNhomSoChe);
        }

        // DELETE: api/BoTriNhomSoChes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBoTriNhomSoChe(string id)
        {
            if (_context.BoTriNhomSoChe == null)
            {
                return NotFound();
            }
            var boTriNhomSoChe = await _context.BoTriNhomSoChe.FindAsync(id);
            if (boTriNhomSoChe == null)
            {
                return NotFound();
            }

            _context.BoTriNhomSoChe.Remove(boTriNhomSoChe);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BoTriNhomSoCheExists(string id)
        {
            return (_context.BoTriNhomSoChe?.Any(e => e.MaNhanVien == id)).GetValueOrDefault();
        }


        [HttpGet("{dateTime}/{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> LoadBoTriNhomSoChe(string dateTime, string xuongId)
        {
            if (_context.BoTriNhomSoChe == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmBoTriNhomSoChe.LoadBoTriNhomSoChe(date1, xuongId);
            return items;
        }

        [HttpGet("{xuongId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetNhomSoChe(string xuongId)
        {
            if (_context.BoTriNhomSoChe == null)
            {
                return NotFound();
            }
            var items = Vm.VmNhomSoCheDinhHinh.Gets(xuongId);
            return items;
        }
        [HttpGet("{dateTime}/{xuongId}/{nhomSoChe}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> NhomSoCheSelectionChanged(string dateTime, string xuongId, string nhomSoChe)
        {
            if (_context.BoTriNhomSoChe == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmNhomSoCheDinhHinh.NhomSoCheSelectionChanged(date1, xuongId, nhomSoChe);
            return items;
        }
        [HttpPost("{dateTime}/{xuongId}/{nhomSoChe}")]
        [Authorize]
        public async Task<IActionResult> MoveTo(string dateTime, string xuongId, string nhomSoChe, Tuple<string> dataT)
        {
            try
            {
                DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                if (_context.NhanVienDaiThanh == null)
                {
                    return NotFound();
                }
                Vm.VmBoTriNhomSoChe.MoveTo(date1,xuongId,nhomSoChe, dataT);
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
        [HttpPost("{dateTime}/{xuongId}/{nhomSoChe}")]
        [Authorize]
        public async Task<IActionResult> Remove(string dateTime, string xuongId, string nhomSoChe, Tuple<string> dataT)
        {
            try
            {
                DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                if (_context.NhanVienDaiThanh == null)
                {
                    return NotFound();
                }
                Vm.VmBoTriNhomSoChe.Remove(date1,xuongId,nhomSoChe, dataT);
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
        [HttpPost("{dateTime}/{xuongId}/{nhomSoChe}")]
        [Authorize]
        public async Task<IActionResult> Add(string dateTime, string xuongId, string nhomSoChe, Tuple<string> dataT)
        {
            try
            {
                DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                if (_context.NhanVienDaiThanh == null)
                {
                    return NotFound();
                }
                Vm.VmBoTriNhomSoChe.Add(date1,xuongId,nhomSoChe, dataT);
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
