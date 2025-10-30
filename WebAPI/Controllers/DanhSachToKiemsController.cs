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
    public class DanhSachToKiemsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public DanhSachToKiemsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/DanhSachToKiems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DanhSachToKiem>>> GetDanhSachToKiem()
        {
          if (_context.DanhSachToKiem == null)
          {
              return NotFound();
          }
            return await _context.DanhSachToKiem.ToListAsync();
        }

        // GET: api/DanhSachToKiems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DanhSachToKiem>> GetDanhSachToKiem(string id)
        {
          if (_context.DanhSachToKiem == null)
          {
              return NotFound();
          }
            var danhSachToKiem = await _context.DanhSachToKiem.FindAsync(id);

            if (danhSachToKiem == null)
            {
                return NotFound();
            }

            return danhSachToKiem;
        }

        // PUT: api/DanhSachToKiems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDanhSachToKiem(string id, DanhSachToKiem danhSachToKiem)
        {
            if (id != danhSachToKiem.MaToKiem)
            {
                return BadRequest();
            }

            _context.Entry(danhSachToKiem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DanhSachToKiemExists(id))
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

        // POST: api/DanhSachToKiems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DanhSachToKiem>> PostDanhSachToKiem(DanhSachToKiem danhSachToKiem)
        {
          if (_context.DanhSachToKiem == null)
          {
              return Problem("Entity set 'dbPMScontext.DanhSachToKiem'  is null.");
          }
            _context.DanhSachToKiem.Add(danhSachToKiem);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (DanhSachToKiemExists(danhSachToKiem.MaToKiem))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetDanhSachToKiem", new { id = danhSachToKiem.MaToKiem }, danhSachToKiem);
        }

        // DELETE: api/DanhSachToKiems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDanhSachToKiem(string id)
        {
            if (_context.DanhSachToKiem == null)
            {
                return NotFound();
            }
            var danhSachToKiem = await _context.DanhSachToKiem.FindAsync(id);
            if (danhSachToKiem == null)
            {
                return NotFound();
            }

            _context.DanhSachToKiem.Remove(danhSachToKiem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DanhSachToKiemExists(string id)
        {
            return (_context.DanhSachToKiem?.Any(e => e.MaToKiem == id)).GetValueOrDefault();
        }
    }
}
