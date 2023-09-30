using Microsoft.ApplicationInsights.DataContracts;

namespace SpyderByteAPI.Middleware
{
    public class RequestBodyToInsightMiddleware : IMiddleware
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
                        request.EnableBuffering();
                        request.Body.Position = 0;

                        using (var reader = new StreamReader(request.Body, leaveOpen: true))
                        {
                            string requestBody  = await reader.ReadToEndAsync();

                            var index = requestBody.IndexOf("Content-Type: image/");
                            if (index != -1)
                            {
                                var endIndex = requestBody.IndexOf("---", index);
                                requestBody.Remove(index, endIndex - index);
                            }

                            requestTelemetry.Properties.Add("RequestBody", requestBody);
                        }

                        request.Body.Position = 0;
                    }
                }
            }

            await next(context!);
        }
    }
}
