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
    public class PD_PhieuXuatChiTietController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public PD_PhieuXuatChiTietController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/PD_PhieuXuatChiTiet
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PD_PhieuXuatChiTiet>>> Gets()
        {
          if (_context.PD_PhieuXuatChiTiet == null)
          {
              return NotFound();
          }
            return await _context.PD_PhieuXuatChiTiet.ToListAsync();
        }

        // GET: api/PD_PhieuXuatChiTiet/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PD_PhieuXuatChiTiet>> Get(string id)
        {
          if (_context.PD_PhieuXuatChiTiet == null)
          {
              return NotFound();
          }
            var pD_PhieuXuatChiTiet = await _context.PD_PhieuXuatChiTiet.FindAsync(id);

            if (pD_PhieuXuatChiTiet == null)
            {
                return NotFound();
            }

            return pD_PhieuXuatChiTiet;
        }

        // PUT: api/PD_PhieuXuatChiTiet/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, PD_PhieuXuatChiTiet pD_PhieuXuatChiTiet)
        {
            if (id != pD_PhieuXuatChiTiet.SoPhieu)
            {
                return BadRequest();
            }

            _context.Entry(pD_PhieuXuatChiTiet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PD_PhieuXuatChiTietExists(id))
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

        // POST: api/PD_PhieuXuatChiTiet
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PD_PhieuXuatChiTiet>> Post(PD_PhieuXuatChiTiet pD_PhieuXuatChiTiet)
        {
          if (_context.PD_PhieuXuatChiTiet == null)
          {
              return Problem("Entity set 'dbPMScontext.PD_PhieuXuatChiTiet'  is null.");
          }
            _context.PD_PhieuXuatChiTiet.Add(pD_PhieuXuatChiTiet);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PD_PhieuXuatChiTietExists(pD_PhieuXuatChiTiet.SoPhieu))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("Get", new { id = pD_PhieuXuatChiTiet.SoPhieu }, pD_PhieuXuatChiTiet);
        }

        // DELETE: api/PD_PhieuXuatChiTiet/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.PD_PhieuXuatChiTiet == null)
            {
                return NotFound();
            }
            var pD_PhieuXuatChiTiet = await _context.PD_PhieuXuatChiTiet.FindAsync(id);
            if (pD_PhieuXuatChiTiet == null)
            {
                return NotFound();
            }

            _context.PD_PhieuXuatChiTiet.Remove(pD_PhieuXuatChiTiet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PD_PhieuXuatChiTietExists(string id)
        {
            return (_context.PD_PhieuXuatChiTiet?.Any(e => e.SoPhieu == id)).GetValueOrDefault();
        }
    }
}
