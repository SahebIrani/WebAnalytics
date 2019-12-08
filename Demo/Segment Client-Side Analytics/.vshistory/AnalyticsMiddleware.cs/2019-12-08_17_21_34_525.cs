using System;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Demo.SegmentClientSideAnalytics
{
    public class AnalyticsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private string _connectionString { get; set; }
        private readonly IConfiguration _configuration;

        public AnalyticsMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory,
            IConfiguration configuration)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<AnalyticsMiddleware>();
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                StringBuilder info = new StringBuilder($"Got a request{Environment.NewLine}---{Environment.NewLine}");
                info.Append($"- remote IP: {context.Connection.RemoteIpAddress}{Environment.NewLine}");
                info.Append($"- path: {context.Request.Path}{Environment.NewLine}");
                info.Append($"- query string: {context.Request.QueryString}{Environment.NewLine}");
                info.Append($"- [headers] ua: {context.Request.Headers[HeaderNames.UserAgent]}{Environment.NewLine}");
                info.Append($"- [headers] referer: {context.Request.Headers[HeaderNames.Referer]}{Environment.NewLine}");
                _logger.LogWarning(info.ToString());
            }
            catch (Exception ex)
            {
                // we don't care much about exceptions in analytics
                _logger.LogError($"Some error in analytics middleware. {ex.Message}");
            }

            await _next(context);
        }
    }

    public static class AnalyticsMiddlewareExtensions
    {
        public static IApplicationBuilder UseAnalytics(this IApplicationBuilder builder) =>
            builder.UseMiddleware<AnalyticsMiddleware>();
    }
}
