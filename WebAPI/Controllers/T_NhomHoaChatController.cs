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
    public class T_NhomHoaChatController : ControllerBase
    {
        private readonly dbPMScontext _context;

        public T_NhomHoaChatController(dbPMScontext context)
        {
            _context = context;
        }
        private MainViewModel Vm => MainViewModel.Instance;
        // GET: api/T_NhomHoaChat
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T_NhomHoaChat>>> Gets()
        {
          if (_context.T_NhomHoaChat == null)
          {
              return NotFound();
          }
            return Vm.VmT_NhomHoaChat.Items;
        }

        // GET: api/T_NhomHoaChat/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T_NhomHoaChat>> Get(string id)
        {
          if (_context.T_NhomHoaChat == null)
          {
              return NotFound();
          }
            var t_NhomHoaChat = Vm.VmT_NhomHoaChat.Find(id);

            if (t_NhomHoaChat == null)
            {
                return NotFound();
            }

            return t_NhomHoaChat;
        }

        // PUT: api/T_NhomHoaChat/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, T_NhomHoaChat t_NhomHoaChat)
        {
            if (id != t_NhomHoaChat.Ma)
            {
                return BadRequest();
            }

            if (!Vm.VmT_NhomHoaChat.Exists(t_NhomHoaChat))
            {
                return NotFound();
            }
            Vm.VmT_NhomHoaChat.Update_Command.Execute(t_NhomHoaChat);
            return NoContent();
        }

        // POST: api/T_NhomHoaChat
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T_NhomHoaChat>> Post(T_NhomHoaChat t_NhomHoaChat)
        {
          if (_context.T_NhomHoaChat == null)
          {
              return Problem("Entity set 'dbPMScontext.T_NhomHoaChat'  is null.");
          }
            if (Vm.VmT_NhomHoaChat.Exists(t_NhomHoaChat))
            {
                return Conflict();
            }
            Vm.VmT_NhomHoaChat.Insert_Command.Execute(t_NhomHoaChat);
            return CreatedAtAction("Get", new { id = t_NhomHoaChat.Ma }, t_NhomHoaChat);
        }

        // DELETE: api/T_NhomHoaChat/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.T_NhomHoaChat == null)
            {
                return NotFound();
            }
            var t_NhomHoaChat = Vm.VmT_NhomHoaChat.Find(id);
            if (t_NhomHoaChat == null)
            {
                return NotFound();
            }

            Vm.VmT_NhomHoaChat.Delete_Command.Execute(t_NhomHoaChat);

            return NoContent();
        }

        private bool T_NhomHoaChatExists(string id)
        {
            return (_context.T_NhomHoaChat?.Any(e => e.Ma == id)).GetValueOrDefault();
        }
    }
}
