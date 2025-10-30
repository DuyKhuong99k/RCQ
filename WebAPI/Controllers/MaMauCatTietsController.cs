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
    public class MaMauCatTietsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaMauCatTietsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaMauCatTiets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaMauCatTiet>>> Gets()
        {
          if (_context.MaMauCatTiet == null)
          {
              return NotFound();
          }
            return await _context.MaMauCatTiet.ToListAsync();
        }

        // GET: api/MaMauCatTiets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaMauCatTiet>> Get(string id)
        {
          if (_context.MaMauCatTiet == null)
          {
              return NotFound();
          }
            var maMauCatTiet = await _context.MaMauCatTiet.FindAsync(id);

            if (maMauCatTiet == null)
            {
                return NotFound();
            }

            return maMauCatTiet;
        }

        // PUT: api/MaMauCatTiets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaMauCatTiet maMauCatTiet)
        {
            if (id != maMauCatTiet.Ma)
            {
                return BadRequest();
            }

            _context.Entry(maMauCatTiet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaMauCatTietExists(id))
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

        // POST: api/MaMauCatTiets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaMauCatTiet>> Post(MaMauCatTiet maMauCatTiet)
        {
          if (_context.MaMauCatTiet == null)
          {
              return Problem("Entity set 'dbPMScontext.MaMauCatTiet'  is null.");
          }
            _context.MaMauCatTiet.Add(maMauCatTiet);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MaMauCatTietExists(maMauCatTiet.Ma))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("Get", new { id = maMauCatTiet.Ma }, maMauCatTiet);
        }

        // DELETE: api/MaMauCatTiets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaMauCatTiet == null)
            {
                return NotFound();
            }
            var maMauCatTiet = await _context.MaMauCatTiet.FindAsync(id);
            if (maMauCatTiet == null)
            {
                return NotFound();
            }

            _context.MaMauCatTiet.Remove(maMauCatTiet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MaMauCatTietExists(string id)
        {
            return (_context.MaMauCatTiet?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
