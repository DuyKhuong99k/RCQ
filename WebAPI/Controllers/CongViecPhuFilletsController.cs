using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Models.Repos.Models;
using ViewModels.Repos.API;

namespace WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CongViecPhuFilletsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public CongViecPhuFilletsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/CongViecPhuFillets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CongViecPhuFillet>>> Gets()
        {
            if (_context.CongViecPhuFillet == null)
            {
                return NotFound();
            }
            return await _context.CongViecPhuFillet.ToListAsync();
        }

        // GET: api/CongViecPhuFillets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CongViecPhuFillet>> Get(string id)
        {
            if (_context.CongViecPhuFillet == null)
            {
                return NotFound();
            }
            var congViecPhuFillet = await _context.CongViecPhuFillet.FindAsync(id);

            if (congViecPhuFillet == null)
            {
                return NotFound();
            }

            return congViecPhuFillet;
        }

        // PUT: api/CongViecPhuFillets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, CongViecPhuFillet congViecPhuFillet)
        {
            if (id != congViecPhuFillet.Ma)
            {
                return BadRequest();
            }

            _context.Entry(congViecPhuFillet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CongViecPhuFilletExists(id))
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

        // POST: api/CongViecPhuFillets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CongViecPhuFillet>> Post(CongViecPhuFillet congViecPhuFillet)
        {
            if (_context.CongViecPhuFillet == null)
            {
                return Problem("Entity set 'dbPMScontext.CongViecPhuFillet'  is null.");
            }
            _context.CongViecPhuFillet.Add(congViecPhuFillet);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CongViecPhuFilletExists(congViecPhuFillet.Ma))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCongViecPhuFillet", new { id = congViecPhuFillet.Ma }, congViecPhuFillet);
        }

        // DELETE: api/CongViecPhuFillets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.CongViecPhuFillet == null)
            {
                return NotFound();
            }
            var congViecPhuFillet = await _context.CongViecPhuFillet.FindAsync(id);
            if (congViecPhuFillet == null)
            {
                return NotFound();
            }

            _context.CongViecPhuFillet.Remove(congViecPhuFillet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CongViecPhuFilletExists(string id)
        {
            return (_context.CongViecPhuFillet?.Any(e => e.Ma == id)).GetValueOrDefault();
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = Vm.VmThanhPhamPhuFillet.Get().ToList();

            return Ok(items);
        }
        [HttpGet("{dateTime}/{xuongId}/{thanhPham}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> LoadPhanBoNhanVienPhuFillet(string dateTime, string xuongId, string thanhPham)
        {
            if (_context.CongViecPhuFillet == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmThanhPhamPhuFillet.ThanhPhamPhuFilletSelectChanged(date1, xuongId, thanhPham);
            return items;
        }
       
        [HttpGet("{dateTime}/{xuongId}/{thanhPham}/{maHoSoInput}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> FindNhanVienByMaHoSo(string dateTime, string xuongId, string thanhPham, string maHoSoInput)
        {
            if (_context.CongViecPhuFillet == null)
            {
                return NotFound();
            }
            DateTime date1 = DateTime.ParseExact(dateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            var items = Vm.VmNhanVien.FindNhanVienByMaHoSo(date1, xuongId, thanhPham, maHoSoInput);
            return items;
        }
    }
}
