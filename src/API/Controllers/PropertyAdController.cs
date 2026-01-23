using Application.Abstracts.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PropertyAdController : ControllerBase
{
    private readonly IRepository<PropertyAd, int> _repo;

    public PropertyAdController(IRepository<PropertyAd, int> repo)
    {
        _repo = repo;
    }

    // GET: api/PropertyAd
    [HttpGet]
    public IActionResult Get()
    {
        var ads = _repo.Query()
            .AsNoTracking()
            .Include(x => x.Media)
            .ToList();

        return Ok(ads);
    }

    // GET: api/PropertyAd/5
    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        var ad = _repo.Query()
            .AsNoTracking()
            .Include(x => x.Media)
            .FirstOrDefault(x => x.Id == id);

        if (ad is null) return NotFound();
        return Ok(ad);
    }

    // POST: api/PropertyAd
    [HttpPost]
    public IActionResult Post([FromBody] PropertyAd propertyAd)
    {
        _repo.Add(propertyAd);
        _repo.SaveChanges();

        return CreatedAtAction(nameof(Get), new { id = propertyAd.Id }, propertyAd);
    }

    // PUT: api/PropertyAd/5
    [HttpPut("{id:int}")]
    public IActionResult Put(int id, [FromBody] PropertyAd propertyAd)
    {
        if (id != propertyAd.Id)
            return BadRequest("ID mismatch");

        _repo.Update(propertyAd);
        _repo.SaveChanges();

        return NoContent();
    }

    // DELETE: api/PropertyAd/5
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var ad = _repo.GetById(id);
        if (ad is null) return NotFound();

        _repo.Delete(ad);
        _repo.SaveChanges();

        return NoContent();
    }
}
