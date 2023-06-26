using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sample.API.Entites;
using Sample.API.Requests;

namespace Sample.API.Controllers;

[Authorize]
[Route("api/[controller]/[action]")]
public class CountryController : ControllerBase
{
    private readonly SampleDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public CountryController(SampleDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _context.Countries.Select(c => new
        {
            c.Id,
            c.Name,
            CityiesCount = c.Cities.Count,
        }).ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromForm] CountryRequest request)
    {
        var country = new Country(request.Name,
            await TryUpload(request.ImageFile));
        _context.Add(country);
        await _context.SaveChangesAsync();
        return Ok(new
        {
            country.Id,
            country.Name,
            country.ImageUrl,
        });
    }

    private async Task<string?> TryUpload(IFormFile? file)
    {
        if (file is null)
        {
            return null;
        }

        var path = Path.Combine(Guid.NewGuid() + file.FileName);
        await file.CopyToAsync(System.IO.File.Create(Path.Combine(_webHostEnvironment.WebRootPath, path)));
        return path;
    }
}