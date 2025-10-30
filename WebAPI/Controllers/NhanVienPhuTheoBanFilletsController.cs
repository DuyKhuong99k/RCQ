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
    public class NhanVienPhuTheoBanFilletsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public NhanVienPhuTheoBanFilletsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/NhanVienPhuTheoBanFillets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NhanVienPhuTheoBanFillet>>> Gets()
        {
            if (_context.NhanVienPhuTheoBanFillet == null)
            {
                return NotFound();
            }
            return await _context.NhanVienPhuTheoBanFillet.ToListAsync();
        }

        // GET: api/NhanVienPhuTheoBanFillets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NhanVienPhuTheoBanFillet>> Get(string id)
        {
            if (_context.NhanVienPhuTheoBanFillet == null)
            {
                return NotFound();
            }
            var nhanVienPhuTheoBanFillet = await _context.NhanVienPhuTheoBanFillet.FindAsync(id);

            if (nhanVienPhuTheoBanFillet == null)
            {
                return NotFound();
            }

            return nhanVienPhuTheoBanFillet;
        }

        // PUT: api/NhanVienPhuTheoBanFillets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, NhanVienPhuTheoBanFillet nhanVienPhuTheoBanFillet)
        {
            if (id != nhanVienPhuTheoBanFillet.MaNhanVien)
            {
                return BadRequest();
            }

            _context.Entry(nhanVienPhuTheoBanFillet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NhanVienPhuTheoBanFilletExists(id))
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

        // POST: api/NhanVienPhuTheoBanFillets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NhanVienPhuTheoBanFillet>> Post(NhanVienPhuTheoBanFillet nhanVienPhuTheoBanFillet)
        {
            if (_context.NhanVienPhuTheoBanFillet == null)
            {
                return Problem("Entity set 'dbPMScontext.NhanVienPhuTheoBanFillet'  is null.");
            }
            _context.NhanVienPhuTheoBanFillet.Add(nhanVienPhuTheoBanFillet);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (NhanVienPhuTheoBanFilletExists(nhanVienPhuTheoBanFillet.MaNhanVien))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetNhanVienPhuTheoBanFillet", new { id = nhanVienPhuTheoBanFillet.MaNhanVien }, nhanVienPhuTheoBanFillet);
        }

        // DELETE: api/NhanVienPhuTheoBanFillets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.NhanVienPhuTheoBanFillet == null)
            {
                return NotFound();
            }
            var nhanVienPhuTheoBanFillet = await _context.NhanVienPhuTheoBanFillet.FindAsync(id);
            if (nhanVienPhuTheoBanFillet == null)
            {
                return NotFound();
            }

            _context.NhanVienPhuTheoBanFillet.Remove(nhanVienPhuTheoBanFillet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NhanVienPhuTheoBanFilletExists(string id)
        {
            return (_context.NhanVienPhuTheoBanFillet?.Any(e => e.MaNhanVien == id)).GetValueOrDefault();
        }
    }
}
