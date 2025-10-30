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
    public class T_KhangSinhController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_KhangSinhController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/T_KhangSinh
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T_KhangSinh>>> Gets()
        {
          if (_context.T_KhangSinh == null)
          {
              return NotFound();
          }
            return Vm.VmT_KhangSinh.Items;
        }

        // GET: api/T_KhangSinh/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T_KhangSinh>> Get(string id)
        {
          if (_context.T_KhangSinh == null)
          {
              return NotFound();
          }
            var t_KhangSinh = Vm.VmT_KhangSinh.Find(id);

            if (t_KhangSinh == null)
            {
                return NotFound();
            }

            return t_KhangSinh;
        }

        // PUT: api/T_KhangSinh/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, T_KhangSinh t_KhangSinh)
        {
            if (id != t_KhangSinh.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmT_KhangSinh.Exists(t_KhangSinh))
            {
                return NotFound();
            }
            Vm.VmT_KhangSinh.Update_Command.Execute(t_KhangSinh);
            return NoContent();
        }

        // POST: api/T_KhangSinh
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_KhangSinh>> Post(T_KhangSinh t_KhangSinh)
        {
          if (_context.T_KhangSinh == null)
          {
              return Problem("Entity set 'dbPMScontext.T_KhangSinh'  is null.");
          }
            if (Vm.VmT_KhangSinh.Exists(t_KhangSinh))
            {
                return Conflict();
            }
            Vm.VmT_KhangSinh.Insert_Command.Execute(t_KhangSinh);
            return CreatedAtAction("Get", new { id = t_KhangSinh.Ma }, t_KhangSinh);
        }

        // DELETE: api/T_KhangSinh/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.T_KhangSinh == null)
            {
                return NotFound();
            }
            var t_KhangSinh = Vm.VmT_KhangSinh.Find(id);
            if (t_KhangSinh == null)
            {
                return NotFound();
            }

            Vm.VmT_KhangSinh.Delete_Command.Execute(t_KhangSinh);

            return NoContent();
        }

        private bool T_KhangSinhExists(string id)
        {
            return (_context.T_KhangSinh?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
