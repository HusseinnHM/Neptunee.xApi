
# xApi - Single line to test your api !
![](https://img.shields.io/nuget/dt/Neptunee.xApi)
[![](https://img.shields.io/nuget/v/Neptunee.xApi)](https://www.nuget.org/packages/Neptunee.xApi)

Easy way to build integration test using xUnit & WebApplicationFactory

## Basic Usage

```csharp
 _fixture.Api.Rout<SampleControllerTest>(nameof(SampleControllerTest.GetAll))
    .FromQuery(nameof(name),name)
    .Send()
    .EnsureSuccessStatusCode();
``` 

* OR :

```csharp
 _fixture.Api.Rout(HttpMethod.Get, "api/something/getbyid")
    .FromRoute(nameof(id),id)
    .Send()
    .EnsureSuccessStatusCode();
```

## Setup

create Fixture class to use it in all tests

```csharp
public class IntegrationTestFixture : IntegrationTest<Startup>
{
    public IntegrationTestFixture()
    {
        Configure(webApplicationBuilder: builder => builder.UseEnvironment(Environments.Development),
            clientBuilder: options => options.AllowAutoRedirect = false);
    }
}
```

Example how to use ```IntegrationTestFixture``` for ```SampleControllerTest``` api testing

```csharp

public class SampleControllerTest : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;


    public SampleControllerTest(IntegrationTestFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _fixture.SetApiOutput(output); // to dispaly some info while sending apis (url , request , response , status code ..)
    }

    [Fact]
    public Task AnyApiTest()
    {
        // logic
    }
}
```

## How To Send Query / Routes

```csharp
var id = Guid.NewGuid();
_fixture.Api.Rout<SampleControllerTest>(nameof(SampleControllerTest.Get))
    .FromQuery(nameof(id),id) // key & value
    .FromQuery(new // anonymos object
    {
        id,
    })
    .FromQuery(new AnyDto() // object like dto, mv, ..etc
    {
        Id = id,
    })
    
```
***same for Routes just use ```FromRoute()``` instead of ```FromQuery()```.***

## How To Send Json / FormData

```csharp
_fixture.Api.Rout<SampleControllerTest>(nameof(SampleControllerTest.Add))
    .FromBody(new AddRequstClass // for Json body request
    {
        Id = Guid.NewGuid(),
        Email = _fixture.Faker.Person.Email,
    })
    .FromForm(new AddRequstClass // for Multipart Form Data body request
    {
        Name = _fixture.Faker.Name.FullName(),
        ImageFile = _fixture.FormFile("AssetsFolder/test.png")
    })    
```

## Assert Status Code

**You can use this methods :**

* ```AssertSuccessStatusCodeAsync()``` / ```AssertSuccessStatusCode()``` when its range 200-299.
* ```AssertNotSuccessStatusCodeAsync()``` / ```AssertNotSuccessStatusCode()``` when its otherwise range 200-299.
* ```AssertStatusCodeEqualsAsync(HttpStatusCode.OK)``` / ```AssertStatusCodeEquals(HttpStatusCode.OK)```
* ```AssertStatusCodeNotEqualsAsync(HttpStatusCode.Conflict)``` / ```AssertStatusCodeNotEquals(HttpStatusCode.Conflict)```

## Working With Files
You have ```FormFile()``` method in ```IntegrationTest<TStartup>``` to help you to send files as IFormFile with 3 overloads :
```csharp
public IFormFile FormFile(string path, string? contentType = null)
```
```csharp
 public IFormFile FormFile(Stream stream, string fileName, string? contentType = null)
```
```csharp
public IFormFile FormFile(byte[] bytes, string fileName, string? contentType = null)
```
**Note:** the optional contentType parameter throw null exception if [TryGetContentType()]("https://github.com/dotnet/aspnetcore/blob/main/src/Middleware/StaticFiles/src/FileExtensionContentTypeProvider.cs#L432")
returns false, means contentType is unknown.

***PS:*** can send empty Files with ```FakeFormFile()``` method.