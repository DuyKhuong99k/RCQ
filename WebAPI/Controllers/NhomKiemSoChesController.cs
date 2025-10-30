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
    public class NhomKiemSoChesController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public NhomKiemSoChesController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/NhomKiemSoChes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NhomKiemSoChe>>> Gets()
        {
          if (_context.NhomKiemSoChe == null)
          {
              return NotFound();
          }
            return await _context.NhomKiemSoChe.ToListAsync();
        }

        // GET: api/NhomKiemSoChes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NhomKiemSoChe>> Get(string id)
        {
          if (_context.NhomKiemSoChe == null)
          {
              return NotFound();
          }
            var nhomKiemSoChe = await _context.NhomKiemSoChe.FindAsync(id);

            if (nhomKiemSoChe == null)
            {
                return NotFound();
            }

            return nhomKiemSoChe;
        }

        // PUT: api/NhomKiemSoChes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, NhomKiemSoChe nhomKiemSoChe)
        {
            if (id != nhomKiemSoChe.Ma)
            {
                return BadRequest();
            }

            _context.Entry(nhomKiemSoChe).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NhomKiemSoCheExists(id))
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

        // POST: api/NhomKiemSoChes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NhomKiemSoChe>> Post(NhomKiemSoChe nhomKiemSoChe)
        {
          if (_context.NhomKiemSoChe == null)
          {
              return Problem("Entity set 'dbPMScontext.NhomKiemSoChe'  is null.");
          }
            _context.NhomKiemSoChe.Add(nhomKiemSoChe);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (NhomKiemSoCheExists(nhomKiemSoChe.Ma))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("Get", new { id = nhomKiemSoChe.Ma }, nhomKiemSoChe);
        }

        // DELETE: api/NhomKiemSoChes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.NhomKiemSoChe == null)
            {
                return NotFound();
            }
            var nhomKiemSoChe = await _context.NhomKiemSoChe.FindAsync(id);
            if (nhomKiemSoChe == null)
            {
                return NotFound();
            }

            _context.NhomKiemSoChe.Remove(nhomKiemSoChe);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NhomKiemSoCheExists(string id)
        {
            return (_context.NhomKiemSoChe?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
