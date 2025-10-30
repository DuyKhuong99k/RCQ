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
    public class TheTuDaiThanhsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public TheTuDaiThanhsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/TheTuDaiThanhs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TheTuDaiThanh>>> Gets()
        {
          if (_context.TheTuDaiThanh == null)
          {
              return NotFound();
          }
            return await _context.TheTuDaiThanh.ToListAsync();
        }

        // GET: api/TheTuDaiThanhs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TheTuDaiThanh>> Get(string id)
        {
          if (_context.TheTuDaiThanh == null)
          {
              return NotFound();
          }
            var theTuDaiThanh = await _context.TheTuDaiThanh.FindAsync(id);

            if (theTuDaiThanh == null)
            {
                return NotFound();
            }

            return theTuDaiThanh;
        }

        // PUT: api/TheTuDaiThanhs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, TheTuDaiThanh theTuDaiThanh)
        {
            if (id != theTuDaiThanh.MaTheTu)
            {
                return BadRequest();
            }

            _context.Entry(theTuDaiThanh).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TheTuDaiThanhExists(id))
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

        // POST: api/TheTuDaiThanhs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TheTuDaiThanh>> Post(TheTuDaiThanh theTuDaiThanh)
        {
          if (_context.TheTuDaiThanh == null)
          {
              return Problem("Entity set 'dbPMScontext.TheTuDaiThanh'  is null.");
          }
            _context.TheTuDaiThanh.Add(theTuDaiThanh);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TheTuDaiThanhExists(theTuDaiThanh.MaTheTu))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTheTuDaiThanh", new { id = theTuDaiThanh.MaTheTu }, theTuDaiThanh);
        }

        // DELETE: api/TheTuDaiThanhs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.TheTuDaiThanh == null)
            {
                return NotFound();
            }
            var theTuDaiThanh = await _context.TheTuDaiThanh.FindAsync(id);
            if (theTuDaiThanh == null)
            {
                return NotFound();
            }

            _context.TheTuDaiThanh.Remove(theTuDaiThanh);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TheTuDaiThanhExists(string id)
        {
            return (_context.TheTuDaiThanh?.Any(e => e.MaTheTu == id)).GetValueOrDefault();
        }
    }
}
