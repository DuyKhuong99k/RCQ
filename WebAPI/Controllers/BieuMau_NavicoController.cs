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
    public class BieuMau_NavicoController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public BieuMau_NavicoController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/BieuMau_Navico
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BieuMau_Navico>>> Gets()
        {
          if (_context.BieuMau_Navico == null)
          {
              return NotFound();
          }
            return await _context.BieuMau_Navico.ToListAsync();
        }

        // GET: api/BieuMau_Navico/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BieuMau_Navico>> Get(string id)
        {
          if (_context.BieuMau_Navico == null)
          {
              return NotFound();
          }
            var bieuMau_Navico = await _context.BieuMau_Navico.FindAsync(id);

            if (bieuMau_Navico == null)
            {
                return NotFound();
            }

            return bieuMau_Navico;
        }

        // PUT: api/BieuMau_Navico/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, BieuMau_Navico bieuMau_Navico)
        {
            if (id != bieuMau_Navico.Id)
            {
                return BadRequest();
            }

            _context.Entry(bieuMau_Navico).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BieuMau_NavicoExists(id))
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

        // POST: api/BieuMau_Navico
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BieuMau_Navico>> Post(BieuMau_Navico bieuMau_Navico)
        {
          if (_context.BieuMau_Navico == null)
          {
              return Problem("Entity set 'dbPMScontext.BieuMau_Navico'  is null.");
          }
            _context.BieuMau_Navico.Add(bieuMau_Navico);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BieuMau_NavicoExists(bieuMau_Navico.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBieuMau_Navico", new { id = bieuMau_Navico.Id }, bieuMau_Navico);
        }

        // DELETE: api/BieuMau_Navico/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.BieuMau_Navico == null)
            {
                return NotFound();
            }
            var bieuMau_Navico = await _context.BieuMau_Navico.FindAsync(id);
            if (bieuMau_Navico == null)
            {
                return NotFound();
            }

            _context.BieuMau_Navico.Remove(bieuMau_Navico);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BieuMau_NavicoExists(string id)
        {
            return (_context.BieuMau_Navico?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
