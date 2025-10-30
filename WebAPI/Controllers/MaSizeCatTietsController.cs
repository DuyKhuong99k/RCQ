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
    public class MaSizeCatTietsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaSizeCatTietsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaSizeCatTiets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaSizeCatTiet>>> Gets()
        {
          if (_context.MaSizeCatTiet == null)
          {
              return NotFound();
          }
            return await _context.MaSizeCatTiet.ToListAsync();
        }

        // GET: api/MaSizeCatTiets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaSizeCatTiet>> Get(string id)
        {
          if (_context.MaSizeCatTiet == null)
          {
              return NotFound();
          }
            var maSizeCatTiet = await _context.MaSizeCatTiet.FindAsync(id);

            if (maSizeCatTiet == null)
            {
                return NotFound();
            }

            return maSizeCatTiet;
        }

        // PUT: api/MaSizeCatTiets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaSizeCatTiet maSizeCatTiet)
        {
            if (id != maSizeCatTiet.Ma)
            {
                return BadRequest();
            }

            _context.Entry(maSizeCatTiet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaSizeCatTietExists(id))
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

        // POST: api/MaSizeCatTiets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaSizeCatTiet>> Post(MaSizeCatTiet maSizeCatTiet)
        {
          if (_context.MaSizeCatTiet == null)
          {
              return Problem("Entity set 'dbPMScontext.MaSizeCatTiet'  is null.");
          }
            _context.MaSizeCatTiet.Add(maSizeCatTiet);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MaSizeCatTietExists(maSizeCatTiet.Ma))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetMaSizeCatTiet", new { id = maSizeCatTiet.Ma }, maSizeCatTiet);
        }

        // DELETE: api/MaSizeCatTiets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaSizeCatTiet == null)
            {
                return NotFound();
            }
            var maSizeCatTiet = await _context.MaSizeCatTiet.FindAsync(id);
            if (maSizeCatTiet == null)
            {
                return NotFound();
            }

            _context.MaSizeCatTiet.Remove(maSizeCatTiet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MaSizeCatTietExists(string id)
        {
            return (_context.MaSizeCatTiet?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
