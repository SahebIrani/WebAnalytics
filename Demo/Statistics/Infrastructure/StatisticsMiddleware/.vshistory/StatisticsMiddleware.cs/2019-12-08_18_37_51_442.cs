using System;
using System.Linq;
using System.Threading.Tasks;

using Demo.Statistics.Infrastructure.AppContext;
using Demo.Statistics.Infrastructure.Extension;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace Demo.Statistics.Infrastructure.StatisticsMiddleware
{
    public class StatisticsMiddleware
    {
        public StatisticsMiddleware(RequestDelegate next) => _next = next;
        private readonly RequestDelegate _next;

        public async Task Invoke(HttpContext context)
        {
            StatisticsAppDbContext statisticsContext = context.RequestServices.GetRequiredService<StatisticsAppDbContext>();
            if (
                    !context.Request.IsAjaxRequest() &&
                    (context.Request.Path.Value.Equals("/") ||
                    !context.Request.Path.Value.StartsWith("/AdminProfile"))
            )
            {
                try
                {
                    Entities.Statistics statistics = new Entities.Statistics
                    {
                        Count = 1,
                        CreateDate = DateTimeOffset.UtcNow,
                        ModifyDate = DateTimeOffset.UtcNow,
                        Browser = new UserAgent.UserAgent(context.Request.Headers["User-Agent"]).Browser.Name,
                        Os = new UserAgent.UserAgent(context.Request.Headers["User-Agent"]).Os.Name,
                        UserAgent = context.Request.Headers["User-Agent"].ToString(),
                        AcceptLanguage = context.Request.Headers["Accept-Language"].ToString(),
                        Host = context.Request.Host.ToString(),
                        IsHTTPS = context.Request.IsHttps.ToString(),
                        Method = context.Request.Method,
                        Path = context.Request.Path,
                        Protocol = context.Request.Protocol,
                        Scheme = context.Request.Scheme,
                        Referer = context.GetHeaderValue("Referer")
                    };
                    if (context.User.Identity.IsAuthenticated)
                    {
                        statistics.UserId = context.User.GetUserId();
                        statistics.IpAddress = GetCurrentIpAddress(context);
                    }
                    else
                    {
                        statistics.UserId = "NotSignIn";
                        statistics.IpAddress = GetCurrentIpAddress(context);
                    }
                    await statisticsContext.Statistics.AddAsync(statistics);
                    await statisticsContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex.InnerException);
                }
                finally
                {
                    statisticsContext.Dispose();
                }
            }
            await _next.Invoke(context);
        }

        private string GetCurrentIpAddress(HttpContext context)
        {
            string result = string.Empty;
            string forwardedHttpHeaderKey = "X-FORWARDED-FOR";
            try
            {
                //first try to get IP address from the forwarded header
                if (context.Request.Headers != null)
                {
                    //the X-Forwarded-For (XFF) HTTP header field is a de facto standard for identifying the originating IP address of a client
                    //connecting to a web server through an HTTP proxy or load balancer

                    StringValues forwardedHeader = context.Request.Headers[forwardedHttpHeaderKey];
                    if (!StringValues.IsNullOrEmpty(forwardedHeader))
                        result = forwardedHeader.FirstOrDefault();
                }

                //if this header not exists try get connection remote IP address
                if (string.IsNullOrEmpty(result) && context.Connection.RemoteIpAddress != null)
                    result = context.Connection.RemoteIpAddress.ToString();
            }
            catch
            {
                return string.Empty;
            }

            //some of the validation
            if (result != null && result.Equals("::1", StringComparison.InvariantCultureIgnoreCase))
                result = "127.0.0.1";

            //remove port
            if (!string.IsNullOrEmpty(result))
                result = result.Split(':').FirstOrDefault();

            return result;
        }

        public static class StatisticsMiddlewareExtensions
        {
            public static IApplicationBuilder UseStatistics(this IApplicationBuilder builder) =>
                builder.UseMiddleware<StatisticsMiddleware>();
        }
    }
}
