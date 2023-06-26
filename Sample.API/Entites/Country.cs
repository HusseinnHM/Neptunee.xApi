namespace Sample.API.Entites;

public class Country : BaseEntity
{
    public Country(string name, string? imageUrl)
    {
        Name = name;
        ImageUrl = imageUrl;
    }

    public string Name { get; set; }
    public string? ImageUrl { get; set; }
    public ICollection<City> Cities { get; set; } = new HashSet<City>();
}