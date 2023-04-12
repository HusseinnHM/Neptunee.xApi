using System.Collections;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Neptunee.xApi;

public static class FormDataHelper
{
    public static MultipartFormDataContent ToFormData<TFormData>(this TFormData formData)
    {
        var json = JsonConvert.SerializeObject(formData, Formatting.Indented, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
        });
        var children = JObject.Parse(json).Children();
        var content = new MultipartFormDataContent();
        content.FormDataRec(children);
        content.FormFileRec(typeof(TFormData), formData!, string.Empty);
        return content;
    }

    private static void FormDataRec(this MultipartFormDataContent content,JEnumerable<JToken> children)
    {
        foreach (var child in children)
        {
            if (child!.Type is not (JTokenType.Property or JTokenType.Array or JTokenType.Object))
            {
                content.Add(child.Path, child.ToString());
            }

            if (!child.HasValues) continue;
            content.FormDataRec(child.Children());
        }
    }

    private static void FormFileRec(this MultipartFormDataContent content, Type type, object obj, string propPath)
    {
        foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.PropertyType != typeof(string)))
        {
            if (prop.PropertyType == typeof(IFormFile) && prop.GetValue(obj) is IFormFile formFile)
            {
                content.Add(formFile, propPath + prop.Name);
            }
            else if (prop.PropertyType.IsGenericType && prop.GetValue(obj) is IEnumerable enumerable)
            {
                var typeOfList = prop.PropertyType.GetGenericArguments().First();
                var i = 0;
                foreach (var e in enumerable)
                {
                    if (typeOfList == typeof(IFormFile))
                    {
                        content.Add((IFormFile)e, $"{propPath}{prop.Name}");
                    }
                    else
                    {
                        FormFileRec(content, typeOfList, e, $"{propPath}{prop.Name}[{i}].");
                    }

                    i++;
                }
            }
            else if (prop.PropertyType.IsClass && prop.GetValue(obj) is { } propObj)
            {
                FormFileRec(content, prop.PropertyType, propObj, $"{propPath}{prop.Name}.");
            }
        }
    }

    public static void Add<TValue>(this MultipartFormDataContent content, string propName, TValue value)
    {
        content.Add(new StringContent(value?.ToString() ?? string.Empty), propName);
    }
    
    private static void Add(this MultipartFormDataContent formDataContent, IFormFile file, string propName)
    {
        var content = ByteArrayContent(file);
        formDataContent.Add(content, propName, file.FileName);
    }

    private static ByteArrayContent ByteArrayContent(IFormFile file)
    {
        var content = new ByteArrayContent(Bytes(file));
        content.Headers.Add(HeaderNames.ContentType, file.ContentType);
        return content;
    }

    private static byte[] Bytes(IFormFile formFile)
    {
        using var memoryStream = new MemoryStream();
        formFile.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }
}