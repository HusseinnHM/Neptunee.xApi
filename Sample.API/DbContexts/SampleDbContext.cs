using Microsoft.EntityFrameworkCore;
using Sample.API.Entites;

namespace Sample.API;

public class SampleDbContext : DbContext
{
    public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options)
    {
        
    }
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<City> Cities => Set<City>();
}