using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Abstractions;

namespace Neptunee.xApi;

public class ApiFactory<TStartup> : IDisposable where TStartup : class
{
    public readonly WebApplicationFactory<TStartup> ApplicationFactory;
    public readonly HttpClient Client;
    private ITestOutputHelper? _output;

    internal static ApiFactory<TStartup> Initial(Action<IWebHostBuilder>? webApplicationBuilder = null,Action<WebApplicationFactoryClientOptions>? clientBuilder = null)
        => new(webApplicationBuilder,clientBuilder);

    private ApiFactory(Action<IWebHostBuilder>? webApplicationBuilder, Action<WebApplicationFactoryClientOptions>? clientBuilder)
    {
        ApplicationFactory = new WebApplicationFactory<TStartup>().WithWebHostBuilder(webApplicationBuilder ?? (_ => { }));
        var clientOptions = new WebApplicationFactoryClientOptions();
        clientBuilder?.Invoke(clientOptions);
        Client = ApplicationFactory.CreateClient(clientOptions);
    }

    internal void SetOutput(ITestOutputHelper output) => _output = output;


    public void JwtAuthenticate(string token)
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public XApiRequest Rout<TController>(string methodName)
        => new(Client, _output , typeof(TController), methodName);


    public XApiRequest Rout(HttpMethod httpMethod, string requestUri)
        => new(Client, _output, httpMethod, requestUri);


    public void Dispose()
    {
        Client.Dispose();
        ApplicationFactory.Dispose();
    }
}