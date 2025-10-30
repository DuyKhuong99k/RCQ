using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Repos;
using Models.Repos.Models;

namespace WebAPI.Controllers
{
     [Route("api/[controller]/[action]")]
    [ApiController]
    public class MayPhanCoesController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public MayPhanCoesController(dbPMScontext context)
        {
            _context = context;
        }

        // GET: api/MayPhanCoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MayPhanCo>>> Gets()
        {
          if (_context.MayPhanCo == null)
          {
              return NotFound();
          }
            return await _context.MayPhanCo.ToListAsync();
        }

        // GET: api/MayPhanCoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MayPhanCo>> Get(string id)
        {
          if (_context.MayPhanCo == null)
          {
              return NotFound();
          }
            var mayPhanCo = await _context.MayPhanCo.FindAsync(id);

            if (mayPhanCo == null)
            {
                return NotFound();
            }

            return mayPhanCo;
        }

        // PUT: api/MayPhanCoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, MayPhanCo mayPhanCo)
        {
            if (id != mayPhanCo.Ma)
            {
                return BadRequest();
            }

            _context.Entry(mayPhanCo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MayPhanCoExists(id))
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

        // POST: api/MayPhanCoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MayPhanCo>> Post(MayPhanCo mayPhanCo)
        {
          if (_context.MayPhanCo == null)
          {
              return Problem("Entity set 'dbPMScontext.MayPhanCo'  is null.");
          }
            _context.MayPhanCo.Add(mayPhanCo);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MayPhanCoExists(mayPhanCo.Ma))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetMayPhanCo", new { id = mayPhanCo.Ma }, mayPhanCo);
        }

        // DELETE: api/MayPhanCoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.MayPhanCo == null)
            {
                return NotFound();
            }
            var mayPhanCo = await _context.MayPhanCo.FindAsync(id);
            if (mayPhanCo == null)
            {
                return NotFound();
            }

            _context.MayPhanCo.Remove(mayPhanCo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MayPhanCoExists(string id)
        {
            return (_context.MayPhanCo?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
