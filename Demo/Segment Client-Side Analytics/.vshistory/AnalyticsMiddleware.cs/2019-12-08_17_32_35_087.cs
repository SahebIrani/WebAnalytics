using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

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
            const string pattern = @"(^\/css\/.*)|(^\/fonts\/.*)|(^\/images\/.*)|(^\/js\/.*)|(^\/lib\/.*)
                                    |(^\/favicon(-.*|.ico$))|(^\/robots.txt$)|(^\/rss.xml$)";
            if (!Regex.IsMatch(context.Request.Path, pattern, RegexOptions.IgnoreCase))
            {
                // only requests for "good" resources get processed here

                try
                {
                    StringBuilder info = new StringBuilder($"Got a request{Environment.NewLine}---{Environment.NewLine}");
                    info.Append($"- remote IP: {context.Connection.RemoteIpAddress}{Environment.NewLine}");
                    info.Append($"- path: {context.Request.Path}{Environment.NewLine}");
                    info.Append($"- query string: {context.Request.QueryString}{Environment.NewLine}");
                    info.Append($"- [headers] ua: {context.Request.Headers[HeaderNames.UserAgent]}{Environment.NewLine}");
                    info.Append($"- [headers] referer: {context.Request.Headers[HeaderNames.Referer]}{Environment.NewLine}");
                    _logger.LogWarning(info.ToString());

                    //Now every time your server(website) gets a request, you’ll see the following in your log:
                    //WARN  [2018-07-27 21:13:19] Got a visitor
                    //---
                    //- remote IP: 127.0.0.1
                    //- path: /some/path
                    //- query string: ?some=param
                    //- [headers] ua: Mozilla/5.0 (Macintosh; Intel Mac OS X 10.13; rv:62.0) Gecko/20100101 Firefox/62.0
                    //- [headers] referer: http://some-source

                    //But such a log-wall is rather hard to analyze, better to store this data in a database instead:
                    using SqlConnection sqlConn = new SqlConnection(_connectionString);
                    sqlConn.Open();
                    SqlCommand cmd = new SqlCommand(
                        "INSERT INTO analytics(dt, ip, path, query, referer, ua)" +
                        " VALUES(@dt, @ip, @path, @query, @referer, @ua);"
                        , sqlConn
                    );
                    cmd.Parameters.AddWithValue("@dt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@ip", context.Connection.RemoteIpAddress);
                    cmd.Parameters.AddWithValue("@path", context.Request.Path);
                    cmd.Parameters.AddWithValue("@query", context.Request.QueryString);
                    cmd.Parameters.AddWithValue("@referer", context.Request.Headers[HeaderNames.Referer]);
                    cmd.Parameters.AddWithValue("@ua", context.Request.Headers[HeaderNames.UserAgent]);
                    cmd.ExecuteNonQuery();

                    //And here’s the table for that:
                    //CREATE TABLE `analytics` (
                    //  `id` bigint(11) unsigned NOT NULL AUTO_INCREMENT,
                    //  `dt` datetime NOT NULL,
                    //  `ip` varchar(45) NOT NULL DEFAULT '',
                    //  `path` varchar(250) NOT NULL DEFAULT '',
                    //  `query` varchar(500) DEFAULT NULL,
                    //  `referer` varchar(500) DEFAULT NULL,
                    //  `ua` varchar(250) NOT NULL,
                    //  PRIMARY KEY (`id`)
                    //);
                }
                catch (Exception ex)
                {
                    // we don't care much about exceptions in analytics
                    _logger.LogError($"Some error in analytics middleware. {ex.Message}");
                }
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
