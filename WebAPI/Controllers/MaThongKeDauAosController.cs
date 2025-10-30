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
    public class MaThongKeDauAosController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MaThongKeDauAosController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/MaThongKeDauAos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaThongKeDauAo>>> Gets()
        {
          if (_context.MaThongKeDauAo == null)
          {
              return NotFound();
          }
            return await _context.MaThongKeDauAo.ToListAsync();
        }

        // GET: api/MaThongKeDauAos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaThongKeDauAo>> Get(string id)
        {
          if (_context.MaThongKeDauAo == null)
          {
              return NotFound();
          }
            var maThongKeDauAo = await _context.MaThongKeDauAo.FindAsync(id);

            if (maThongKeDauAo == null)
            {
                return NotFound();
            }

            return maThongKeDauAo;
        }

        // PUT: api/MaThongKeDauAos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MaThongKeDauAo maThongKeDauAo)
        {
            if (id != maThongKeDauAo.Ma)
            {
                return BadRequest();
            }

            _context.Entry(maThongKeDauAo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaThongKeDauAoExists(id))
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

        // POST: api/MaThongKeDauAos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MaThongKeDauAo>> Post(MaThongKeDauAo maThongKeDauAo)
        {
          if (_context.MaThongKeDauAo == null)
          {
              return Problem("Entity set 'dbPMScontext.MaThongKeDauAo'  is null.");
          }
            _context.MaThongKeDauAo.Add(maThongKeDauAo);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MaThongKeDauAoExists(maThongKeDauAo.Ma))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("Get", new { id = maThongKeDauAo.Ma }, maThongKeDauAo);
        }

        // DELETE: api/MaThongKeDauAos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MaThongKeDauAo == null)
            {
                return NotFound();
            }
            var maThongKeDauAo = await _context.MaThongKeDauAo.FindAsync(id);
            if (maThongKeDauAo == null)
            {
                return NotFound();
            }

            _context.MaThongKeDauAo.Remove(maThongKeDauAo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MaThongKeDauAoExists(string id)
        {
            return (_context.MaThongKeDauAo?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
