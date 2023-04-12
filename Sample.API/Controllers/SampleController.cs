using Microsoft.AspNetCore.Mvc;
using Sample.API.Requests;

namespace Sample.API.Controllers;

[ApiController]
public class SampleController : ControllerBase
{
    [HttpGet("api/[controller]/[action]")]
    public IActionResult Get([FromQuery] Guid id)
    {
        return Ok(new
        {
            Id = id,
            Date = DateTimeOffset.UtcNow,
        });
    }

    [HttpPost("api/[controller]/[action]")]
    public IActionResult Upload([FromForm] UploadRequest request)
    {
        return Ok($"file with name {request.File.FileName} and size {request.File.Length} bytes uploaded to {request.Path}");
    }

    [HttpGet("api/[controller]/[action]")]
    public IActionResult Return400()
    {
        return BadRequest();
    }

    [HttpPost("api/[controller]/[action]/{id:guid}")]
    public IActionResult Modify([FromRoute] Guid id,[FromBody] ModifyRequest request)
    {
        return Ok(new
        {
            id,
            request.Name,
            request.Email,
            Date = DateTimeOffset.UtcNow,
        });
    }
}