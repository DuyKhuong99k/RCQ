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
    public class MaMauXepKhuonsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaMauXepKhuonsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaMauXepKhuons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaMauXepKhuon>>> Gets()
        {
            if (_context.MaMauXepKhuon == null)
            {
                return NotFound();
            }
            return await _context.MaMauXepKhuon.ToListAsync();
        }

        // GET: api/MaMauXepKhuons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaMauXepKhuon>> Get(string id)
        {
            if (_context.MaMauXepKhuon == null)
            {
                return NotFound();
            }
            var maMauXepKhuon = await _context.MaMauXepKhuon.FindAsync(id);

            if (maMauXepKhuon == null)
            {
                return NotFound();
            }

            return maMauXepKhuon;
        }

        // PUT: api/MaMauXepKhuons/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaMauXepKhuon maMauXepKhuon)
        {
            if (id != maMauXepKhuon.Ma)
            {
                return BadRequest();
            }

            _context.Entry(maMauXepKhuon).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaMauXepKhuonExists(id))
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

        // POST: api/MaMauXepKhuons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaMauXepKhuon>> Post(MaMauXepKhuon maMauXepKhuon)
        {
            if (_context.MaMauXepKhuon == null)
            {
                return Problem("Entity set 'dbPMScontext.MaMauXepKhuon'  is null.");
            }
            _context.MaMauXepKhuon.Add(maMauXepKhuon);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MaMauXepKhuonExists(maMauXepKhuon.Ma))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetMaMauXepKhuon", new { id = maMauXepKhuon.Ma }, maMauXepKhuon);
        }

        // DELETE: api/MaMauXepKhuons/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaMauXepKhuon == null)
            {
                return NotFound();
            }
            var maMauXepKhuon = await _context.MaMauXepKhuon.FindAsync(id);
            if (maMauXepKhuon == null)
            {
                return NotFound();
            }

            _context.MaMauXepKhuon.Remove(maMauXepKhuon);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MaMauXepKhuonExists(string id)
        {
            return (_context.MaMauXepKhuon?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetAlls()
        {
            var items = _context.MaMauXepKhuon.OrderByDescending(x => x.Ma).ToList();

            return Ok(items);
        }
    }
}
