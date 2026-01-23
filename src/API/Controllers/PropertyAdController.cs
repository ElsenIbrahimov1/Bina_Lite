using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PropertyAdController : ControllerBase
{
    private readonly BinaLiteDbContext _context;

    public PropertyAdController(BinaLiteDbContext context)
    {
        _context = context;
    }

    // GET: api/PropertyAd
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var ads = await _context.PropertyAds
            .AsNoTracking()
            .Include(x => x.Media)
            .ToListAsync();

        return Ok(ads);
    }

    // GET: api/PropertyAd/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var ad = await _context.PropertyAds
            .AsNoTracking()
            .Include(x => x.Media)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (ad == null)
            return NotFound();

        return Ok(ad);
    }

    // POST: api/PropertyAd
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PropertyAd propertyAd)
    {
        await _context.PropertyAds.AddAsync(propertyAd);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = propertyAd.Id }, propertyAd);
    }

    // PUT: api/PropertyAd/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] PropertyAd propertyAd)
    {
        if (id != propertyAd.Id)
            return BadRequest("ID mismatch");

        _context.PropertyAds.Update(propertyAd);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/PropertyAd/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ad = await _context.PropertyAds.FindAsync(id);

        if (ad == null)
            return NotFound();

        _context.PropertyAds.Remove(ad);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
