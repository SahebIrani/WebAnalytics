using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Demo.Statistics.Infrastructure.AppContext;
using Demo.Statistics.Infrastructure.Extension;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

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
                    !context.IsAjaxRequest() &&
                    (context.Path.Value.Equals("/") ||
                    !context.Path.Value.StartsWith("/AdminProfile"))
            )
            {
                try
                {
                    HttpRequest ctxReq = context.Request;
                    RequestHeaders header = ctxReq.GetTypedHeaders();
                    Uri uriReferer = header.Referer;
                    string referer = ctxReq.Headers[HeaderNames.Referer].ToString();
                    string userId = context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

                    Entities.Statistics statistics = new Entities.Statistics
                    {
                        Count = 1,
                        CreateDate = DateTimeOffset.UtcNow,
                        ModifyDate = DateTimeOffset.UtcNow,
                        Browser = new UserAgent.UserAgent(ctxReq.Headers[HeaderNames.UserAgent]).Browser.Name,
                        Os = new UserAgent.UserAgent(ctxReq.Headers[HeaderNames.UserAgent]).Os.Name,
                        UserAgent = ctxReq.Headers[HeaderNames.UserAgent].ToString(),
                        AcceptLanguage = ctxReq.Headers[HeaderNames.AcceptLanguage].ToString(),
                        Host = ctxReq.Host.ToString(),
                        IsHTTPS = ctxReq.IsHttps.ToString(),
                        Method = ctxReq.Method,
                        Path = ctxReq.Path,
                        Protocol = ctxReq.Protocol,
                        Scheme = ctxReq.Scheme,
                        Referer = uriReferer.AbsoluteUri
                    };
                    if (context.User.Identity.IsAuthenticated)
                    {
                        statistics.UserId = userId;
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
                if (ctxReq.Headers != null)
                {
                    //the X-Forwarded-For (XFF) HTTP header field is a de facto standard for identifying the originating IP address of a client
                    //connecting to a web server through an HTTP proxy or load balancer

                    StringValues forwardedHeader = ctxReq.Headers[forwardedHttpHeaderKey];
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
    }
}
