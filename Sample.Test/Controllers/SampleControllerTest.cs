using System.Net;
using Bogus;
using Neptunee.xApi;
using Sample.API.Controllers;
using Sample.API.Requests;
using Xunit.Abstractions;

namespace Sample.Test.Controllers;

public class SampleControllerTest : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;

    public SampleControllerTest(IntegrationTestFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _fixture.SetApiOutput(output);
    }

    [Fact]
    public async Task Get()
    {
        var id = Guid.NewGuid();

        await _fixture.Api.Rout<SampleController>(nameof(SampleController.Get))
            .FromQuery(new
            {
                id
            })
            .SendAsync()
            .AssertSuccessStatusCodeAsync();
    }

    [Fact]
    public async Task Return400()
    {
        await _fixture.Api.Rout<SampleController>(nameof(SampleController.Return400))
            .SendAsync()
            .AssertNotSuccessStatusCodeAsync();
    }

    [Fact]
    public async Task Upload()
    {
        var request = new UploadRequest
        {
            Path = Path.Combine("media", "images"),
            File = _fixture.FormFile(Const.SvgImagePath)
        };

        await _fixture.Api.Rout<SampleController>(nameof(SampleController.Upload))
            .FromForm(request)
            .SendAsync()
            .AssertStatusCodeEqualsAsync(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Modify()
    {
        var id = Guid.NewGuid();
        var request = new ModifyRequest
        {
            Name = _fixture.Faker.Person.FullName,
            Email = _fixture.Faker.Person.Email,
        };

        await _fixture.Api.Rout<SampleController>(nameof(SampleController.Modify))
            .FromRoute(nameof(id), id)
            .FromBody(request)
            .SendAsync()
            .AssertSuccessStatusCodeAsync();
    }
}