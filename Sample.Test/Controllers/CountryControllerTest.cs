using Neptunee.xApi;
using Sample.API.Controllers;
using Sample.API.Entites;
using Sample.API.Requests;
using Xunit.Abstractions;

namespace Sample.Test.Controllers;

public class CountryControllerTest : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;

    public CountryControllerTest(IntegrationTestFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _fixture.SetApiOutput(output);
    }

    [Fact]
    public async Task GetAll()
    {
        _fixture.DefaultAuth();
        
        await _fixture.Api.Rout<CountryController>(nameof(CountryController.GetAll))
            .SendAsync()
            .AssertSuccessStatusCodeAsync();
    }

    [Fact]
    public async Task Add()
    {
        _fixture.DefaultAuth();
        
        var request = new CountryRequest
        {
            Name = _fixture.Faker.Address.Country(),
            ImageFile = _fixture.FormFile(Const.SvgImagePath)
        };

        await _fixture.Api.Rout<CountryController>(nameof(CountryController.Add))
            .FromForm(request)
            .SendAsync()
            .AssertSuccessStatusCodeAsync();
    }
}