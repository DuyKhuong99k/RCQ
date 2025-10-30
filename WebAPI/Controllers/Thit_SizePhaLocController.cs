using Microsoft.AspNetCore.Mvc;
using Models.Repos;
using Models.Repos.Models;
using ViewModels.Repos.API;

namespace WebAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class Thit_SizePhaLocController : ControllerBase
{
    private readonly dbPMScontext _context;

    public Thit_SizePhaLocController(dbPMScontext context)
    {
        _context = context;
    }

    private MainViewModel Vm => MainViewModel.Instance;

    // DELETE: api/Thit_SizePhaLoc/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (_context.Thit_SizePhaLoc == null) return NotFound();

        var thit_SizePhaLoc = Vm.VmThit_SizePhaLoc.Find(id);
        if (thit_SizePhaLoc == null) return NotFound();

        Vm.VmThit_SizePhaLoc.Delete_Command.Execute(thit_SizePhaLoc);

        return NoContent();
    }

    // GET: api/Thit_SizePhaLoc/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Thit_SizePhaLoc>> Get(string id)
    {
        if (_context.Thit_SizePhaLoc == null) return NotFound();

        var thit_SizePhaLoc = Vm.VmThit_SizePhaLoc.Find(id);

        if (thit_SizePhaLoc == null) return NotFound();

        return thit_SizePhaLoc;
    }

    // GET: api/Thit_SizePhaLoc
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Thit_SizePhaLoc>>> Gets()
    {
        if (_context.Thit_SizePhaLoc == null) return NotFound();

        return Vm.VmThit_SizePhaLoc.Items;
    }

    // POST: api/Thit_SizePhaLoc
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Thit_SizePhaLoc>> Post(Thit_SizePhaLoc thit_SizePhaLoc)
    {
        if (_context.Thit_SizePhaLoc == null) return Problem("Entity set 'dbPMScontext.Thit_SizePhaLoc'  is null.");
        _context.Thit_SizePhaLoc.Add(thit_SizePhaLoc);
        if (Vm.VmThit_SizePhaLoc.Exists(thit_SizePhaLoc)) return Conflict();
        Vm.VmThit_SizePhaLoc.Insert_Command.Execute(thit_SizePhaLoc);
        return CreatedAtAction("Get", new { id = thit_SizePhaLoc.Ma }, thit_SizePhaLoc);
    }

    // PUT: api/Thit_SizePhaLoc/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, Thit_SizePhaLoc thit_SizePhaLoc)
    {
        if (id != thit_SizePhaLoc.Ma) return BadRequest();

        if (!Vm.VmThit_SizePhaLoc.Exists(thit_SizePhaLoc)) return NotFound();
        Vm.VmThit_SizePhaLoc.Update_Command.Execute(thit_SizePhaLoc);

        return NoContent();
    }
}