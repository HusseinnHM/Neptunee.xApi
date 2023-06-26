namespace Sample.API.Entites;

public class City : BaseEntity
{
    public City(string name, Guid countryId)
    {
        Name = name;
        CountryId = countryId;
    }

    public string Name { get; set; }
    public Guid CountryId { get; set; }
    public Country Country { get; set; }
}