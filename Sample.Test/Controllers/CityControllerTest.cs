using Neptunee.xApi;
using Sample.API;
using Sample.API.Controllers;
using Sample.API.Entites;
using Sample.API.Requests;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Attributes;

namespace Sample.Test.Controllers;

public class CityControllerTest : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;

    public CityControllerTest(IntegrationTestFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _fixture.SetApiOutput(output);
    }

    [Fact]
    public async Task GetAll()
    {
        await _fixture.Api.Rout<CityController>(nameof(CityController.GetAll))
            .SendAsync()
            .AssertSuccessStatusCodeAsync();
    }

    [Fact]
    public async Task Add()
    {
        var request = new CityRequest
        {
            Name = _fixture.Faker.Address.City(),
            CountryId = _fixture.GetId<Country>()
        };

        await _fixture.Api.Rout<CityController>(nameof(CityController.Add))
            .FromBody(request)
            .SendAsync()
            .AssertSuccessStatusCodeAsync();
    }

    [Fact]
    public async Task Modify()
    {
        var id = _fixture.GetId<City>();
        var request = new CityRequest
        {
            Name = _fixture.Faker.Address.City(),
            CountryId = _fixture.GetId<Country>()
        };

        await _fixture.Api.Rout<CityController>(nameof(CityController.Modify))
            .FromRoute(nameof(id),id)
            .FromBody(request)
            .SendAsync()
            .AssertSuccessStatusCodeAsync();
    }  
    [Fact]
    public async Task Delete()
    {
        var id = _fixture.GetId<City>();
        
        await _fixture.Api.Rout<CityController>(nameof(CityController.Delete))
            .FromQuery(nameof(id),id)
            .SendAsync()
            .AssertSuccessStatusCodeAsync();
    }
}