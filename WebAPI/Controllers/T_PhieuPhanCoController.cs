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
    public class T_PhieuPhanCoController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_PhieuPhanCoController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/T_PhieuPhanCo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T_PhieuPhanCo>>> Gets()
        {
          if (_context.T_PhieuPhanCo == null)
          {
              return NotFound();
          }
            return Vm.VmT_PhieuPhanCo.Items;
        }

        // GET: api/T_PhieuPhanCo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T_PhieuPhanCo>> Get(string id)
        {
          if (_context.T_PhieuPhanCo == null)
          {
              return NotFound();
          }
            var t_PhieuPhanCo = Vm.VmT_PhieuPhanCo.Find(id);

            if (t_PhieuPhanCo == null)
            {
                return NotFound();
            }

            return t_PhieuPhanCo;
        }

        // PUT: api/T_PhieuPhanCo/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, T_PhieuPhanCo t_PhieuPhanCo)
        {
            if (id != t_PhieuPhanCo.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmT_PhieuPhanCo.Exists(t_PhieuPhanCo))
            {
                return NotFound();
            }
            Vm.VmT_PhieuPhanCo.Update_Command.Execute(t_PhieuPhanCo);
            return NoContent();
        }

        // POST: api/T_PhieuPhanCo
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_PhieuPhanCo>> Post(T_PhieuPhanCo t_PhieuPhanCo)
        {
          if (_context.T_PhieuPhanCo == null)
          {
              return Problem("Entity set 'dbPMScontext.T_PhieuPhanCo'  is null.");
          }
            if (Vm.VmT_PhieuPhanCo.Exists(t_PhieuPhanCo))
            {
                return Conflict();
            }
            Vm.VmT_PhieuPhanCo.Insert_Command.Execute(t_PhieuPhanCo);
            return CreatedAtAction("Get", new { id = t_PhieuPhanCo.Ma }, t_PhieuPhanCo);
        }

        // DELETE: api/T_PhieuPhanCo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.T_PhieuPhanCo == null)
            {
                return NotFound();
            }
            var t_PhieuPhanCo = Vm.VmT_PhieuPhanCo.Find(id);
            if (t_PhieuPhanCo == null)
            {
                return NotFound();
            }

            Vm.VmT_PhieuPhanCo.Delete_Command.Execute(t_PhieuPhanCo);

            return NoContent();
        }

        private bool T_PhieuPhanCoExists(string id)
        {
            return (_context.T_PhieuPhanCo?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
