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
    public class BanCatTietTheoLinesController : ControllerBase
    {
        private readonly dbPMScontext _context;
        
        public BanCatTietTheoLinesController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/BanCatTietTheoLines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BanCatTietTheoLine>>> Gets()
        {
          if (_context.BanCatTietTheoLine == null)
          {
              return NotFound();
          }
            return await _context.BanCatTietTheoLine.ToListAsync();
        }

        // GET: api/BanCatTietTheoLines/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BanCatTietTheoLine>> GetBanCatTietTheoLine(string id)
        {
          if (_context.BanCatTietTheoLine == null)
          {
              return NotFound();
          }
            var banCatTietTheoLine = await _context.BanCatTietTheoLine.FindAsync(id);

            if (banCatTietTheoLine == null)
            {
                return NotFound();
            }

            return banCatTietTheoLine;
        }

        // PUT: api/BanCatTietTheoLines/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBanCatTietTheoLine(string id, BanCatTietTheoLine banCatTietTheoLine)
        {
            if (id != banCatTietTheoLine.MaBanCatTiet)
            {
                return BadRequest();
            }

            _context.Entry(banCatTietTheoLine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BanCatTietTheoLineExists(id))
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

        // POST: api/BanCatTietTheoLines
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BanCatTietTheoLine>> PostBanCatTietTheoLine(BanCatTietTheoLine banCatTietTheoLine)
        {
          if (_context.BanCatTietTheoLine == null)
          {
              return Problem("Entity set 'dbPMScontext.BanCatTietTheoLine'  is null.");
          }
            _context.BanCatTietTheoLine.Add(banCatTietTheoLine);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BanCatTietTheoLineExists(banCatTietTheoLine.MaBanCatTiet))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBanCatTietTheoLine", new { id = banCatTietTheoLine.MaBanCatTiet }, banCatTietTheoLine);
        }

        // DELETE: api/BanCatTietTheoLines/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBanCatTietTheoLine(string id)
        {
            if (_context.BanCatTietTheoLine == null)
            {
                return NotFound();
            }
            var banCatTietTheoLine = await _context.BanCatTietTheoLine.FindAsync(id);
            if (banCatTietTheoLine == null)
            {
                return NotFound();
            }

            _context.BanCatTietTheoLine.Remove(banCatTietTheoLine);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BanCatTietTheoLineExists(string id)
        {
            return (_context.BanCatTietTheoLine?.Any(e => e.MaBanCatTiet == id)).GetValueOrDefault();
        }
    }
}
