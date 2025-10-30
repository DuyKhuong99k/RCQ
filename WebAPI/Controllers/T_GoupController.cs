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
    public class T_GoupController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_GoupController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/T_Goup
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T_Goup>>> Gets()
        {
          if (_context.T_Goup == null)
          {
              return NotFound();
          }
            return Vm.VmT_Goup.Items;
        }

        // GET: api/T_Goup/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T_Goup>> Get(string id)
        {
          if (_context.T_Goup == null)
          {
              return NotFound();
          }
            var t_Goup = Vm.VmT_Goup.Find(id);

            if (t_Goup == null)
            {
                return NotFound();
            }

            return t_Goup;
        }

        // PUT: api/T_Goup/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, T_Goup t_Goup)
        {
            if (id != t_Goup.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmT_Goup.Exists(t_Goup))
            {
                return NotFound();
            }
            Vm.VmT_Goup.Update_Command.Execute(t_Goup);
            return NoContent();
        }

        // POST: api/T_Goup
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_Goup>> Post(T_Goup t_Goup)
        {
          if (_context.T_Goup == null)
          {
              return Problem("Entity set 'dbPMScontext.T_Goup'  is null.");
          }
            if (Vm.VmT_Goup.Exists(t_Goup))
            {
                return Conflict();
            }
            Vm.VmT_Goup.Insert_Command.Execute(t_Goup);
            return CreatedAtAction("Get", new { id = t_Goup.Ma }, t_Goup);
        }

        // DELETE: api/T_Goup/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.T_Goup == null)
            {
                return NotFound();
            }
            var t_Goup = Vm.VmT_Goup.Find(id);
            if (t_Goup == null)
            {
                return NotFound();
            }

            Vm.VmT_Goup.Delete_Command.Execute(t_Goup);

            return NoContent();
        }

        private bool T_GoupExists(string id)
        {
            return (_context.T_Goup?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
