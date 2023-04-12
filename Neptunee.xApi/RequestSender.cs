using System.Net.Http.Json;
using Neptunee.xApi;
using Xunit.Abstractions;

namespace Neptunee.xApi;

public partial class XApiRequest
{
    public async Task<HttpResponseMessage> SendAsync()
    {
        if (_testOutputHelper is null)
        {
            return await _client.SendAsync(_request);
        }

        _testOutputHelper!.WriteLine($"[{_request.Method}] {_request.RequestUri}{Environment.NewLine}");
        PrintRequest();
        var response = await _client.SendAsync(_request);
        PrintResponse(response);
        return response;
    }

    public HttpResponseMessage Send()
    {
        return SendAsync().GetAwaiter().GetResult();
    }


    private void PrintRequest()
    {
        if (_request.Content is not null)
        {
            _testOutputHelper!.WriteLine($"Request :");
            Print(_request.Content);
        }
    }

    private void PrintResponse(HttpResponseMessage response)
    {
        _testOutputHelper!.WriteLine($"Response :");
        Print(response.Content);
        _testOutputHelper.WriteLine($"StatusCode : {(int)response.StatusCode} ({response.StatusCode})");
    }


    private void Print(HttpContent httpContent)
    {
        _testOutputHelper!.WriteLine(httpContent switch
        {
            JsonContent jsonContent => jsonContent.AsString(),
            MultipartFormDataContent multipartFormDataContent => multipartFormDataContent.AsString(),
            { } => httpContent.AsString(),
            _ => string.Empty
        } + Environment.NewLine);
    }
}