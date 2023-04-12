using System.Net;
using Xunit;

namespace Neptunee.xApi;

public static class HttpResponseHelper
{
    public static HttpResponseMessage AssertSuccessStatusCode(this HttpResponseMessage response)
    {
        Assert.True(response.IsSuccessStatusCode);
        return response;
    }

    public static async Task<HttpResponseMessage> AssertSuccessStatusCodeAsync(this Task<HttpResponseMessage> taskResponse)
    {
        return (await taskResponse).AssertSuccessStatusCode();
    }

    public static HttpResponseMessage AssertNotSuccessStatusCode(this HttpResponseMessage response)
    {
        Assert.True(!response.IsSuccessStatusCode);
        return response;
    }

    public static async Task<HttpResponseMessage> AssertNotSuccessStatusCodeAsync(this Task<HttpResponseMessage> taskResponse)
    {
        return (await taskResponse).AssertNotSuccessStatusCode();
    }

    public static HttpResponseMessage AssertStatusCodeEquals(this HttpResponseMessage response, HttpStatusCode statusCode)
    {
        Assert.True(response.StatusCode == statusCode);
        return response;
    }

    public static async Task<HttpResponseMessage> AssertStatusCodeEqualsAsync(this Task<HttpResponseMessage> taskResponse, HttpStatusCode statusCode)
    {
        return (await taskResponse).AssertStatusCodeEquals(statusCode);
    }

    public static HttpResponseMessage AssertStatusCodeNotEquals(this HttpResponseMessage response, HttpStatusCode statusCode)
    {
        Assert.True(response.StatusCode != statusCode);
        return response;
    }

    public static async Task<HttpResponseMessage> AssertStatusCodeNotEqualsAsync(this Task<HttpResponseMessage> taskResponse, HttpStatusCode statusCode)
    {
        return (await taskResponse).AssertStatusCodeNotEquals(statusCode);
    }
}