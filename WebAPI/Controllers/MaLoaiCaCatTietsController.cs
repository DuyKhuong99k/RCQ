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
    public class MaLoaiCaCatTietsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaLoaiCaCatTietsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaLoaiCaCatTiets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaLoaiCaCatTiet>>> Gets()
        {
          if (_context.MaLoaiCaCatTiet == null)
          {
              return NotFound();
          }
            return await _context.MaLoaiCaCatTiet.ToListAsync();
        }

        // GET: api/MaLoaiCaCatTiets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaLoaiCaCatTiet>> Get(string id)
        {
          if (_context.MaLoaiCaCatTiet == null)
          {
              return NotFound();
          }
            var maLoaiCaCatTiet = await _context.MaLoaiCaCatTiet.FindAsync(id);

            if (maLoaiCaCatTiet == null)
            {
                return NotFound();
            }

            return maLoaiCaCatTiet;
        }

        // PUT: api/MaLoaiCaCatTiets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaLoaiCaCatTiet maLoaiCaCatTiet)
        {
            if (id != maLoaiCaCatTiet.Ma)
            {
                return BadRequest();
            }

            _context.Entry(maLoaiCaCatTiet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaLoaiCaCatTietExists(id))
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

        // POST: api/MaLoaiCaCatTiets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaLoaiCaCatTiet>> Post(MaLoaiCaCatTiet maLoaiCaCatTiet)
        {
          if (_context.MaLoaiCaCatTiet == null)
          {
              return Problem("Entity set 'dbPMScontext.MaLoaiCaCatTiet'  is null.");
          }
            _context.MaLoaiCaCatTiet.Add(maLoaiCaCatTiet);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MaLoaiCaCatTietExists(maLoaiCaCatTiet.Ma))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("Get", new { id = maLoaiCaCatTiet.Ma }, maLoaiCaCatTiet);
        }

        // DELETE: api/MaLoaiCaCatTiets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaLoaiCaCatTiet == null)
            {
                return NotFound();
            }
            var maLoaiCaCatTiet = await _context.MaLoaiCaCatTiet.FindAsync(id);
            if (maLoaiCaCatTiet == null)
            {
                return NotFound();
            }

            _context.MaLoaiCaCatTiet.Remove(maLoaiCaCatTiet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MaLoaiCaCatTietExists(string id)
        {
            return (_context.MaLoaiCaCatTiet?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
