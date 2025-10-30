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
    public class BanQuyenPhanMemsController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public BanQuyenPhanMemsController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/BanQuyenPhanMems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BanQuyenPhanMem>>> Gets()
        {
          if (_context.BanQuyenPhanMem == null)
          {
              return NotFound();
          }
            return await _context.BanQuyenPhanMem.ToListAsync();
        }

        // GET: api/BanQuyenPhanMems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BanQuyenPhanMem>> Get(string id)
        {
          if (_context.BanQuyenPhanMem == null)
          {
              return NotFound();
          }
            var banQuyenPhanMem = await _context.BanQuyenPhanMem.FindAsync(id);

            if (banQuyenPhanMem == null)
            {
                return NotFound();
            }

            return banQuyenPhanMem;
        }

        // PUT: api/BanQuyenPhanMems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, BanQuyenPhanMem banQuyenPhanMem)
        {
            if (id != banQuyenPhanMem.MaMayTinh)
            {
                return BadRequest();
            }

            _context.Entry(banQuyenPhanMem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BanQuyenPhanMemExists(id))
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

        // POST: api/BanQuyenPhanMems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BanQuyenPhanMem>> Post(BanQuyenPhanMem banQuyenPhanMem)
        {
          if (_context.BanQuyenPhanMem == null)
          {
              return Problem("Entity set 'dbPMScontext.BanQuyenPhanMem'  is null.");
          }
            _context.BanQuyenPhanMem.Add(banQuyenPhanMem);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BanQuyenPhanMemExists(banQuyenPhanMem.MaMayTinh))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBanQuyenPhanMem", new { id = banQuyenPhanMem.MaMayTinh }, banQuyenPhanMem);
        }

        // DELETE: api/BanQuyenPhanMems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.BanQuyenPhanMem == null)
            {
                return NotFound();
            }
            var banQuyenPhanMem = await _context.BanQuyenPhanMem.FindAsync(id);
            if (banQuyenPhanMem == null)
            {
                return NotFound();
            }

            _context.BanQuyenPhanMem.Remove(banQuyenPhanMem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BanQuyenPhanMemExists(string id)
        {
            return (_context.BanQuyenPhanMem?.Any(e => e.MaMayTinh == id)).GetValueOrDefault();
        }
    }
}
