using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;
using Xunit.Abstractions;

namespace Neptunee.xApi;

public class IntegrationTest<TStartup> : IDisposable where TStartup : class
{
    public ApiFactory<TStartup> Api { get; protected set; }
    protected IntegrationTest()
    {
        Api = ApiFactory<TStartup>.Initial();
    }

    protected IntegrationTest(ITestOutputHelper output) : this()
        => SetApiOutput(output);

    public void SetApiOutput(ITestOutputHelper output)
        => Api.SetOutput(output);

    protected void Configure(Action<IWebHostBuilder>? webApplicationBuilder = null, Action<WebApplicationFactoryClientOptions>? clientBuilder = null)
        => Api = ApiFactory<TStartup>.Initial(webApplicationBuilder, clientBuilder);


    public IFormFile FakeFormFile(string fileName, string? contentType = null)
        => FormFile(new MemoryStream(), fileName, contentType);

    public IFormFile FormFile(string path, string? fileName = null, string? contentType = null)
        => FormFile(new FileStream(path, FileMode.Open), fileName ?? Path.GetFileName(path), contentType);

    public IFormFile FormFile(byte[] bytes, string fileName, string? contentType = null)
        => FormFile(new MemoryStream(bytes), fileName, contentType);

    public IFormFile FormFile(Stream stream,
        string fileName,
        string? contentType = null)
    {
        if (contentType is null & !new FileExtensionContentTypeProvider().TryGetContentType(fileName, out contentType))
        {
            throw new ArgumentNullException(nameof(contentType), "Must set value, contentType is unknown.");
        }

        return new FormFile(stream, stream.Position, stream.Length, string.Empty, fileName)
        {
            Headers = new HeaderDictionary
            {
                new(HeaderNames.ContentType,contentType) 
            }
        };
    }

    public void Dispose()
    {
        Api.Dispose();
    }
}