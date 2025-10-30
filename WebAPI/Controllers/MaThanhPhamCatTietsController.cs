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
    public class MaThanhPhamCatTietsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaThanhPhamCatTietsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaThanhPhamCatTiets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaThanhPhamCatTiet>>> Gets()
        {
          if (_context.MaThanhPhamCatTiet == null)
          { 
              return NotFound();
          }
            return await _context.MaThanhPhamCatTiet.ToListAsync();
        }

        // GET: api/MaThanhPhamCatTiets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaThanhPhamCatTiet>> Get(string id)
        {
          if (_context.MaThanhPhamCatTiet == null)
          {
              return NotFound();
          }
            var maThanhPhamCatTiet = await _context.MaThanhPhamCatTiet.FindAsync(id);

            if (maThanhPhamCatTiet == null)
            {
                return NotFound();
            }

            return maThanhPhamCatTiet;
        }

        // PUT: api/MaThanhPhamCatTiets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaThanhPhamCatTiet maThanhPhamCatTiet)
        {
            if (id != maThanhPhamCatTiet.MaCa)
            {
                return BadRequest();
            }

            _context.Entry(maThanhPhamCatTiet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaThanhPhamCatTietExists(id))
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

        // POST: api/MaThanhPhamCatTiets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaThanhPhamCatTiet>> Post(MaThanhPhamCatTiet maThanhPhamCatTiet)
        {
          if (_context.MaThanhPhamCatTiet == null)
          {
              return Problem("Entity set 'dbPMScontext.MaThanhPhamCatTiet'  is null.");
          }
            _context.MaThanhPhamCatTiet.Add(maThanhPhamCatTiet);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MaThanhPhamCatTietExists(maThanhPhamCatTiet.MaCa))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("Get", new { id = maThanhPhamCatTiet.MaCa }, maThanhPhamCatTiet);
        }

        // DELETE: api/MaThanhPhamCatTiets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaThanhPhamCatTiet == null)
            {
                return NotFound();
            }
            var maThanhPhamCatTiet = await _context.MaThanhPhamCatTiet.FindAsync(id);
            if (maThanhPhamCatTiet == null)
            {
                return NotFound();
            }

            _context.MaThanhPhamCatTiet.Remove(maThanhPhamCatTiet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MaThanhPhamCatTietExists(string id)
        {
            return (_context.MaThanhPhamCatTiet?.Any(e => e.MaCa == id)).GetValueOrDefault();
        }
    }
}
