using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sample.API.Entites;
using Sample.API.Requests;

namespace Sample.API.Controllers;

[Route("api/[controller]/[action]")]
public class CityController : ControllerBase
{
    private readonly SampleDbContext _context;

    public CityController(SampleDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _context.Cities.Select(c => new
        {
            c.Id,
            c.Name,
            CountryName = c.Country.Name,
        }).ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CityRequest request)
    {
        var city = new City(request.Name, request.CountryId);
        _context.Add(city);
        await _context.SaveChangesAsync();
        return Ok(new
        {
            city.Id,
            city.Name,
            city.CountryId,
        });
    }

    [HttpPost("{id:guid}")]
    public async Task<IActionResult> Modify([FromRoute] Guid id, [FromBody] CityRequest request)
    {
        var city = await _context.Cities.FirstAsync(c => c.Id == id);
        city.Name = request.Name;
        city.CountryId = request.CountryId;
        await _context.SaveChangesAsync();
        return Ok(new
        {
            city.Id,
            city.Name,
            city.CountryId,
        });
    }

    [HttpPost]
    public async Task<IActionResult> Delete([FromQuery] Guid id)
    {
        await _context.Cities.Where(c => c.Id == id).ExecuteDeleteAsync();
        return Ok();
    }
}