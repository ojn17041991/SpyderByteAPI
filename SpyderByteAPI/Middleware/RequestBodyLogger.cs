using Microsoft.ApplicationInsights.DataContracts;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace SpyderByteAPI.Middleware
{
    public class RequestBodyLogger : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context?.Request != null)
            {
                var request = context!.Request;

                var requestTelemetry = context!.Features.Get<RequestTelemetry>();
                if (requestTelemetry != null)
                {
                    if (request.ContentLength > 0 && (request.Method == HttpMethods.Post || request.Method == HttpMethods.Put || request.Method == HttpMethods.Patch))
                    {
                        var properties = new Dictionary<string, string>();

                        request.EnableBuffering();
                        request.Body.Position = 0;

                        using (var reader = new StreamReader(request.Body, leaveOpen: true))
                        {
                            string requestBody  = await reader.ReadToEndAsync();

                            var regex = new Regex(@"; name=""[A-Za-z0-9_]+""");
                            var matches = regex.Matches(requestBody);

                            foreach (var match in matches)
                            {
                                var matchValue = match?.ToString();
                                if (matchValue == null) continue;

                                var nameStr = @"; name=""";
                                var nameStartIdx = matchValue.IndexOf(nameStr) + nameStr.Length;
                                var nameEndIdx = matchValue.IndexOf(@"""", nameStartIdx);
                                var name = matchValue.Substring(nameStartIdx, nameEndIdx - nameStartIdx);

                                var valueStr = @$"{nameStr}{name}""";
                                var valueStartIdx = requestBody.IndexOf(valueStr) + valueStr.Length;
                                string value;

                                if (name.Contains("image"))
                                {
                                    var fileNameStr = @"filename=""";
                                    var fileNameStartIdx = requestBody.IndexOf(fileNameStr, valueStartIdx) + fileNameStr.Length;
                                    var fileNameEndIdx = requestBody.IndexOf(@"""", fileNameStartIdx);
                                    value = requestBody.Substring(fileNameStartIdx, fileNameEndIdx - fileNameStartIdx);
                                }
                                else
                                {
                                    var valueEndIdx = requestBody.IndexOf(@"---", valueStartIdx);
                                    value = requestBody.Substring(valueStartIdx, valueEndIdx - valueStartIdx).Trim();
                                }

                                properties.Add(name, value);
                            }
                        }

                        var propertiesJson = JsonConvert.SerializeObject(properties, Formatting.Indented);
                        requestTelemetry.Properties.Add("requestBody", propertiesJson);

                        request.Body.Position = 0;
                    }
                }
            }

            await next(context!);
        }
    }
}
