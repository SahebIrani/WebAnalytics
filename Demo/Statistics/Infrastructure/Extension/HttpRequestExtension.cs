using Microsoft.AspNetCore.Http;

namespace Demo.Statistics.Infrastructure.Extension
{
    public static class HttpRequestExtension
    {
        private const string RequestedWithHeader = "X-Requested-With";
        private const string XmlHttpRequest = "XMLHttpRequest";

        public static bool IsAjaxRequest(this HttpRequest request)
            => request?.Headers != null ? request.Headers[RequestedWithHeader] == XmlHttpRequest : false;
    }
}
