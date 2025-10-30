using Microsoft.AspNetCore.Mvc;
using Models.Repos;
using Models.Repos.Models;
using ViewModels.Repos.API;

namespace WebAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class BoTriLoSizeThanhPhamsController : ControllerBase
{
    private readonly dbPMScontext _context;

    public BoTriLoSizeThanhPhamsController(dbPMScontext context)
    {
        _context = context;
    }

    private MainViewModel Vm => MainViewModel.Instance;

    // DELETE: api/BoTriLoSizeThanhPhams/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (_context.BoTriLoSizeThanhPham == null) { return NotFound(); }

        var boTriLoSizeThanhPham = Vm.VmBoTriLoSizeThanhPham.Find(id);
        if (boTriLoSizeThanhPham == null) { return NotFound(); }

        Vm.VmBoTriLoSizeThanhPham.Delete_Command.Execute(boTriLoSizeThanhPham);

        return NoContent();
    }

    private bool Exists(int id)
    {
        return Vm.VmBoTriLoSizeThanhPham.Items.Any(x => x.Id == id);
    }

    // GET: api/BoTriLoSizeThanhPhams/5
    [HttpGet("{id}")]
    public async Task<ActionResult<BoTriLoSizeThanhPham>> Get(int id)
    {
        if (_context.BoTriLoSizeThanhPham == null) { return NotFound(); }

        var boTriLoSizeThanhPham = Vm.VmBoTriLoSizeThanhPham.Find(id);

        if (boTriLoSizeThanhPham == null) { return NotFound(); }

        return boTriLoSizeThanhPham;
    }

    // GET: api/BoTriLoSizeThanhPhams
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BoTriLoSizeThanhPham>>> Gets()
    {
        if (_context.BoTriLoSizeThanhPham == null) { return NotFound(); }

        return Vm.VmBoTriLoSizeThanhPham.Items;
    }

    // POST: api/BoTriLoSizeThanhPhams
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<BoTriLoSizeThanhPham>> Post(
        BoTriLoSizeThanhPham boTriLoSizeThanhPham)
    {
        if (_context.BoTriLoSizeThanhPham == null)
            return Problem("Entity set 'dbPMScontext.BoTriLoSizeThanhPham'  is null.");
        if (Vm.VmBoTriLoSizeThanhPham.Exists(boTriLoSizeThanhPham))
        {
            return Conflict();
        }
        Vm.VmBoTriLoSizeThanhPham.Insert_Command.Execute(boTriLoSizeThanhPham);

        return CreatedAtAction("Get", new { id = boTriLoSizeThanhPham.Id }, boTriLoSizeThanhPham);
    }

    // PUT: api/BoTriLoSizeThanhPhams/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, BoTriLoSizeThanhPham boTriLoSizeThanhPham)
    {
        if (id != boTriLoSizeThanhPham.Id) { return BadRequest(); }

        if (!Vm.VmBoTriLoSizeThanhPham.Exists(boTriLoSizeThanhPham))
        {
            return NotFound();
        }
            
        Vm.VmBoTriLoSizeThanhPham.Update_Command.Execute(boTriLoSizeThanhPham);

        return NoContent();
    }
}