using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    public class T_BonController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_BonController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/T_Bon
        [HttpGet]
        [Authorize("AdminPolicy")]// Chỉ cho phép Admin truy cập
        public async Task<ActionResult<IEnumerable<T_Bon>>> Gets()
        {
          if (_context.T_Bon == null)
          {
              return NotFound();
          }
            return Vm.VmT_Bon.Items;
        }

        // GET: api/T_Bon/5
        [HttpGet("{id}")]
         [Authorize("UserPolicy")] // Cho phép cả User truy cập
        public async Task<ActionResult<T_Bon>> Get(string id)
        {
          if (_context.T_Bon == null)
          {
              return NotFound();
          }
            var t_Bon = Vm.VmT_Bon.Find(id);

            if (t_Bon == null)
            {
                return NotFound();
            }

            return t_Bon;
        }

        // PUT: api/T_Bon/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, T_Bon t_Bon)
        {
            if (id != t_Bon.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmT_Bon.Exists(t_Bon))
            {
                return NotFound();
            }
            Vm.VmT_Bon.Update_Command.Execute(t_Bon);
            return NoContent();
        }

        // POST: api/T_Bon
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_Bon>> Post(T_Bon t_Bon)
        {
          if (_context.T_Bon == null)
          {
              return Problem("Entity set 'dbPMScontext.T_Bon'  is null.");
          }
            if (Vm.VmT_Bon.Exists(t_Bon))
            {
                return Conflict();
            }
            Vm.VmT_Bon.Insert_Command.Execute(t_Bon);
            return CreatedAtAction("Get", new { id = t_Bon.Ma }, t_Bon);
        }

        // DELETE: api/T_Bon/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.T_Bon == null)
            {
                return NotFound();
            }
            var t_Bon = Vm.VmT_Bon.Find(id);
            if (t_Bon == null)
            {
                return NotFound();
            }

            Vm.VmT_Bon.Delete_Command.Execute(t_Bon);

            return NoContent();
        }

        private bool T_BonExists(string id)
        {
            return (_context.T_Bon?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
