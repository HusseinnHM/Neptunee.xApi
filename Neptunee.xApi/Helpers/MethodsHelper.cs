using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Neptunee.xApi;

public static class HelperMethods
{
    public static string AsString(this MultipartFormDataContent content)
    {
        var fileNames = content.Where(c => c is ByteArrayContent and not StringContent { Headers.ContentDisposition.Name.Length: > 0 })
            .Select(a => a.Headers.ContentDisposition!.Name!.Trim('\"') + ".");
        var values = content
            .OfType<StringContent>()
            .Where(c => !fileNames.Any(f => c.Headers.ContentDisposition?.Name?.Contains(f) ?? false))
            .Select(a => $"{a.Headers.ContentDisposition?.Name?.Trim('\"')}:{a.ReadAsStringAsync().GetAwaiter().GetResult()}");
        return string.Join(Environment.NewLine, values);
    }
    
    public static string AsString(this JsonContent content)
        => JsonConvert.SerializeObject(content.Value, Formatting.Indented);

    public static string AsString(this HttpContent content)
    {
        var str =  content.ReadAsStringAsync().GetAwaiter().GetResult();
        try
        {
            return JsonContent.Create(JsonConvert.DeserializeObject(str)).AsString(); 
        }
        catch (Exception)
        {
            // ignored
        }

        return str;
    }

  


    public static TAttribute? GetFirstOrDefaultAttr<TAttribute>(this MemberInfo memberInfo, bool inherit = false) where TAttribute : Attribute =>
        memberInfo.GetCustomAttributes<TAttribute>(inherit).FirstOrDefault();

    public static TAttribute? GetFirstOrDefaultAttr<TAttribute>(this Type type, bool inherit = false) where TAttribute : Attribute =>
        ((MemberInfo)type).GetFirstOrDefaultAttr<TAttribute>(inherit);


    public static string FixControllerName(this string s, string controllerName) =>
        s.Replace("[controller]", controllerName, StringComparison.OrdinalIgnoreCase);

    public static string FixActionName(this string s, string actionName) =>
        s.Replace("[action]", actionName, StringComparison.OrdinalIgnoreCase);

    public static string RemoveControllerSuffix(this string s)
    {
        const string controller = nameof(controller);
        return s.EndsWith(controller, StringComparison.OrdinalIgnoreCase)
            ? s.Remove(s.Length - controller.Length, controller.Length)
            : s;
    }
}