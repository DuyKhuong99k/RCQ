using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class NhanVienSanLuongTheoLineFilletsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public NhanVienSanLuongTheoLineFilletsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/NhanVienSanLuongTheoLineFillets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NhanVienSanLuongTheoLineFillet>>> Gets()
        {
          if (_context.NhanVienSanLuongTheoLineFillet == null)
          {
              return NotFound();
          }
            return await _context.NhanVienSanLuongTheoLineFillet.ToListAsync();
        }

        // GET: api/NhanVienSanLuongTheoLineFillets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NhanVienSanLuongTheoLineFillet>> Get(string id)
        {
          if (_context.NhanVienSanLuongTheoLineFillet == null)
          {
              return NotFound();
          }
            var nhanVienSanLuongTheoLineFillet = await _context.NhanVienSanLuongTheoLineFillet.FindAsync(id);

            if (nhanVienSanLuongTheoLineFillet == null)
            {
                return NotFound();
            }

            return nhanVienSanLuongTheoLineFillet;
        }

        // PUT: api/NhanVienSanLuongTheoLineFillets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, NhanVienSanLuongTheoLineFillet nhanVienSanLuongTheoLineFillet)
        {
            if (id != nhanVienSanLuongTheoLineFillet.MaNhanVien)
            {
                return BadRequest();
            }

            _context.Entry(nhanVienSanLuongTheoLineFillet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NhanVienSanLuongTheoLineFilletExists(id))
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

        // POST: api/NhanVienSanLuongTheoLineFillets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NhanVienSanLuongTheoLineFillet>> Post(NhanVienSanLuongTheoLineFillet nhanVienSanLuongTheoLineFillet)
        {
          if (_context.NhanVienSanLuongTheoLineFillet == null)
          {
              return Problem("Entity set 'dbPMScontext.NhanVienSanLuongTheoLineFillet'  is null.");
          }
            _context.NhanVienSanLuongTheoLineFillet.Add(nhanVienSanLuongTheoLineFillet);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (NhanVienSanLuongTheoLineFilletExists(nhanVienSanLuongTheoLineFillet.MaNhanVien))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetNhanVienSanLuongTheoLineFillet", new { id = nhanVienSanLuongTheoLineFillet.MaNhanVien }, nhanVienSanLuongTheoLineFillet);
        }

        // DELETE: api/NhanVienSanLuongTheoLineFillets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.NhanVienSanLuongTheoLineFillet == null)
            {
                return NotFound();
            }
            var nhanVienSanLuongTheoLineFillet = await _context.NhanVienSanLuongTheoLineFillet.FindAsync(id);
            if (nhanVienSanLuongTheoLineFillet == null)
            {
                return NotFound();
            }

            _context.NhanVienSanLuongTheoLineFillet.Remove(nhanVienSanLuongTheoLineFillet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NhanVienSanLuongTheoLineFilletExists(string id)
        {
            return (_context.NhanVienSanLuongTheoLineFillet?.Any(e => e.MaNhanVien == id)).GetValueOrDefault();
        }
    }
}
