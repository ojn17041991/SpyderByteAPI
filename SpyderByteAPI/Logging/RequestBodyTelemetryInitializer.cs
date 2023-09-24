using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace SpyderByteAPI.Logging
{
    public class RequestBodyTelemetryInitializer : ITelemetryInitializer
    {
        readonly IHttpContextAccessor httpContextAccessor;

        public RequestBodyTelemetryInitializer(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public void Initialize(ITelemetry telemetry)
        {
            var requestTelemetry = telemetry as RequestTelemetry;
            if (requestTelemetry == null) return;

            var jsonBodyPropName = "jsonBody";
            if (requestTelemetry.Properties.ContainsKey(jsonBodyPropName)) return;

            var request = httpContextAccessor?.HttpContext?.Request;
            if (request == null) return;

            switch (request.Method)
            {
                case "POST":
                case "PUT":
                case "PATCH":

                    request.EnableBuffering();

                    using (var reader = new StreamReader(request.Body))
                    {
                        string requestBody = reader.ReadToEnd();
                        requestTelemetry.Properties.Add(jsonBodyPropName, requestBody);
                    }

                    request.Body.Position = 0;

                    break;
            }
        }
    }
}
