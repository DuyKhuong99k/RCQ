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
    public class NhanVienPhucVuTheoBansController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public NhanVienPhucVuTheoBansController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/NhanVienPhucVuTheoBans
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NhanVienPhucVuTheoBan>>> Gets()
        {
          if (_context.NhanVienPhucVuTheoBan == null)
          {
              return NotFound();
          }
            return await _context.NhanVienPhucVuTheoBan.ToListAsync();
        }

        // GET: api/NhanVienPhucVuTheoBans/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NhanVienPhucVuTheoBan>> Get(string id)
        {
          if (_context.NhanVienPhucVuTheoBan == null)
          {
              return NotFound();
          }
            var nhanVienPhucVuTheoBan = await _context.NhanVienPhucVuTheoBan.FindAsync(id);

            if (nhanVienPhucVuTheoBan == null)
            {
                return NotFound();
            }

            return nhanVienPhucVuTheoBan;
        }

        // PUT: api/NhanVienPhucVuTheoBans/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, NhanVienPhucVuTheoBan nhanVienPhucVuTheoBan)
        {
            if (id != nhanVienPhucVuTheoBan.MaNhanVien)
            {
                return BadRequest();
            }

            _context.Entry(nhanVienPhucVuTheoBan).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NhanVienPhucVuTheoBanExists(id))
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

        // POST: api/NhanVienPhucVuTheoBans
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NhanVienPhucVuTheoBan>> Post(NhanVienPhucVuTheoBan nhanVienPhucVuTheoBan)
        {
          if (_context.NhanVienPhucVuTheoBan == null)
          {
              return Problem("Entity set 'dbPMScontext.NhanVienPhucVuTheoBan'  is null.");
          }
            _context.NhanVienPhucVuTheoBan.Add(nhanVienPhucVuTheoBan);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (NhanVienPhucVuTheoBanExists(nhanVienPhucVuTheoBan.MaNhanVien))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetNhanVienPhucVuTheoBan", new { id = nhanVienPhucVuTheoBan.MaNhanVien }, nhanVienPhucVuTheoBan);
        }

        // DELETE: api/NhanVienPhucVuTheoBans/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.NhanVienPhucVuTheoBan == null)
            {
                return NotFound();
            }
            var nhanVienPhucVuTheoBan = await _context.NhanVienPhucVuTheoBan.FindAsync(id);
            if (nhanVienPhucVuTheoBan == null)
            {
                return NotFound();
            }

            _context.NhanVienPhucVuTheoBan.Remove(nhanVienPhucVuTheoBan);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NhanVienPhucVuTheoBanExists(string id)
        {
            return (_context.NhanVienPhucVuTheoBan?.Any(e => e.MaNhanVien == id)).GetValueOrDefault();
        }
    }
}
