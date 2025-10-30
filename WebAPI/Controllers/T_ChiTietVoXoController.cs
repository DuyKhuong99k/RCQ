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
    public class T_ChiTietVoXoController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_ChiTietVoXoController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/T_ChiTietVoXo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T_ChiTietVoXo>>> Gets()
        {
          if (_context.T_ChiTietVoXo == null)
          {
              return NotFound();
          }
            return await _context.T_ChiTietVoXo.ToListAsync();
        }

        // GET: api/T_ChiTietVoXo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T_ChiTietVoXo>> Get(string id)
        {
          if (_context.T_ChiTietVoXo == null)
          {
              return NotFound();
          }
            var t_ChiTietVoXo = await _context.T_ChiTietVoXo.FindAsync(id);

            if (t_ChiTietVoXo == null)
            {
                return NotFound();
            }

            return t_ChiTietVoXo;
        }

        // PUT: api/T_ChiTietVoXo/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, T_ChiTietVoXo t_ChiTietVoXo)
        {
            if (id != t_ChiTietVoXo.Id)
            {
                return BadRequest();
            }

            _context.Entry(t_ChiTietVoXo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!T_ChiTietVoXoExists(id))
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

        // POST: api/T_ChiTietVoXo
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_ChiTietVoXo>> Post(T_ChiTietVoXo t_ChiTietVoXo)
        {
          if (_context.T_ChiTietVoXo == null)
          {
              return Problem("Entity set 'dbPMScontext.T_ChiTietVoXo'  is null.");
          }
            _context.T_ChiTietVoXo.Add(t_ChiTietVoXo);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (T_ChiTietVoXoExists(t_ChiTietVoXo.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("Get", new { id = t_ChiTietVoXo.Id }, t_ChiTietVoXo);
        }

        // DELETE: api/T_ChiTietVoXo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.T_ChiTietVoXo == null)
            {
                return NotFound();
            }
            var t_ChiTietVoXo = await _context.T_ChiTietVoXo.FindAsync(id);
            if (t_ChiTietVoXo == null)
            {
                return NotFound();
            }

            _context.T_ChiTietVoXo.Remove(t_ChiTietVoXo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool T_ChiTietVoXoExists(string id)
        {
            return (_context.T_ChiTietVoXo?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
