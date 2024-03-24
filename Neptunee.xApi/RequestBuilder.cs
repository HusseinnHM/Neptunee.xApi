using System.Collections;
using System.Net.Http.Json;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Neptunee.xApi;
using Xunit.Abstractions;

namespace Neptunee.xApi;

public partial class XApiRequest
{
    private readonly ITestOutputHelper? _testOutputHelper;
    private readonly HttpClient _client;
    private readonly HttpRequestMessage _request;

    private string Url
    {
        get => _request.RequestUri?.OriginalString ?? string.Empty;
        set => _request.RequestUri = new(value, UriKind.RelativeOrAbsolute);
    }

    internal XApiRequest(HttpClient client, ITestOutputHelper? testOutputHelper, Type controllerType, string methodName)
    {
        _client = client;
        _testOutputHelper = testOutputHelper;
        var methodInfo = controllerType.GetMethod(methodName)!;
        _request = new HttpRequestMessage(HttpMethod(methodInfo), RequestUri(controllerType, methodInfo));
    }

    internal XApiRequest(HttpClient client, ITestOutputHelper? testOutputHelper, HttpMethod httpMethod, string requestUri)
    {
        _client = client;
        _testOutputHelper = testOutputHelper;
        _request = new HttpRequestMessage(httpMethod, requestUri);
    }

    public XApiRequest FromQuery<TValue>(string key, TValue value)
    {
        var queryParams = Url.Contains("?", StringComparison.Ordinal) ? "&" : "?";
        queryParams += value is not string and IEnumerable enumerable
            ? string.Join("&", enumerable.Cast<dynamic>().Select(v => $"{key}={v}"))
            : $"{key}={value}";
        Url += queryParams;
        return this;
    }

    public XApiRequest FromQuery<TObj>(TObj obj)
    {
        SetProps(obj, p => FromQuery(p.Name, p.GetValue(obj)));
        return this;
    }

    public XApiRequest FromHeader<TValue>(string key, TValue value)
    {
        _request.Headers.Add(key, value!.ToString());
        return this;
    }

    public XApiRequest FromHeader<TObj>(TObj obj)
    {
        SetProps(obj, p => FromHeader(p.Name, p.GetValue(obj)));
        return this;
    }

    public XApiRequest FromRoute<TValue>(string key, TValue value)
    {
        var start = Url.IndexOf("{" + key, StringComparison.OrdinalIgnoreCase);
        var end = Url.IndexOf('}', start);
        Url = Url.Remove(start, end - start + 1).Insert(start, value?.ToString() ?? string.Empty);
        return this;
    }

    public XApiRequest FromRoute<TObj>(TObj obj)
    {
        SetProps(obj, p => FromRoute(p.Name, p.GetValue(obj)));
        return this;
    }


    public XApiRequest FromForm<TForm>(TForm form)
    {
        _request.Content = form.ToFormData();
        return this;
    }

    public XApiRequest FromBody<TBody>(TBody body)
    {
        _request.Content = JsonContent.Create(body);
        return this;
    }

    private void SetProps<TObj>(TObj obj, Action<PropertyInfo> action)
        => typeof(TObj)
            .GetProperties()
            .Where(p => p.GetValue(obj) != null)
            .ToList()
            .ForEach(action);

    private string RequestUri(Type controllerType, MethodInfo methodInfo)
    {
        var controllerName = controllerType.Name.RemoveControllerSuffix();
        var actionName = ActionName(methodInfo);
        var template = RoutTemplate(controllerType, methodInfo);
        return template.FixControllerName(controllerName).FixActionName(actionName);
    }

    private string ActionName(MethodInfo methodInfo)
    {
        var actionNameAttr = methodInfo.GetCustomAttribute<ActionNameAttribute>(true);
        return actionNameAttr?.Name ?? methodInfo.Name;
    }

    private string RoutTemplate(Type controllerType, MethodInfo methodInfo)
    {
        var controllerRoutAttr = controllerType.GetFirstOrDefaultAttr<RouteAttribute>(true);
        var methodRoutAttr = methodInfo.GetFirstOrDefaultAttr<RouteAttribute>();
        var httpMethodAttribute = methodInfo.GetFirstOrDefaultAttr<HttpMethodAttribute>();
        return string.Join("/", new[]
        {
            controllerRoutAttr?.Template,
            methodRoutAttr?.Template,
            httpMethodAttribute?.Template
        }.Where(t => t != null));
    }

    private HttpMethod HttpMethod(MethodInfo methodInfo)
    {
        var httpMethodAttribute = methodInfo.GetFirstOrDefaultAttr<HttpMethodAttribute>();
        return new HttpMethod(httpMethodAttribute?.HttpMethods.FirstOrDefault() ?? "get");
    }
}