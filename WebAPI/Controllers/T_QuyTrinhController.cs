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
    public class T_QuyTrinhController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_QuyTrinhController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/T_QuyTrinh
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T_QuyTrinh>>> Gets()
        {
          if (_context.T_QuyTrinh == null)
          {
              return NotFound();
          }
            return Vm.VmT_QuyTrinh.Items;
        }

        // GET: api/T_QuyTrinh/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T_QuyTrinh>> Get(string id)
        {
          if (_context.T_QuyTrinh == null)
          {
              return NotFound();
          }
            var t_QuyTrinh = Vm.VmT_QuyTrinh.Find(id);

            if (t_QuyTrinh == null)
            {
                return NotFound();
            }

            return t_QuyTrinh;
        }

        // PUT: api/T_QuyTrinh/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, T_QuyTrinh t_QuyTrinh)
        {
            if (id != t_QuyTrinh.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmT_QuyTrinh.Exists(t_QuyTrinh))
            {
                return NotFound();
            }
            Vm.VmT_QuyTrinh.Update_Command.Execute(t_QuyTrinh);
            return NoContent();
        }

        // POST: api/T_QuyTrinh
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_QuyTrinh>> Post(T_QuyTrinh t_QuyTrinh)
        {
          if (_context.T_QuyTrinh == null)
          {
              return Problem("Entity set 'dbPMScontext.T_QuyTrinh'  is null.");
          }
            if (Vm.VmT_QuyTrinh.Exists(t_QuyTrinh))
            {
                return Conflict();
            }
            Vm.VmT_QuyTrinh.Insert_Command.Execute(t_QuyTrinh);
            return CreatedAtAction("Get", new { id = t_QuyTrinh.Ma }, t_QuyTrinh);
        }

        // DELETE: api/T_QuyTrinh/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.T_QuyTrinh == null)
            {
                return NotFound();
            }
            var t_QuyTrinh = Vm.VmT_QuyTrinh.Find(id);
            if (t_QuyTrinh == null)
            {
                return NotFound();
            }

            Vm.VmT_QuyTrinh.Delete_Command.Execute(t_QuyTrinh);

            return NoContent();
        }

        private bool T_QuyTrinhExists(string id)
        {
            return (_context.T_QuyTrinh?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
