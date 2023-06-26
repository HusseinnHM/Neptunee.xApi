namespace Sample.API.Requests;

public class CountryRequest
{
    public string Name { get; set; }
    public IFormFile? ImageFile { get; set; }
}