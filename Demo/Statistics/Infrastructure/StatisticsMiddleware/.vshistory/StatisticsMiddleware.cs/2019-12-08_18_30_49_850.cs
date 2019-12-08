using System;
using System.Linq;
using System.Threading.Tasks;

using Demo.Statistics.Infrastructure.AppContext;
using Demo.Statistics.Infrastructure.Extension;

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
                (context.Request.Path.Value.Equals("/") || context.Request.Path.Value.StartsWith("/Details"))
            )
            //if (!context.Request.IsAjaxRequest()
            //		&& (!context.Request.Path.Value.StartsWith("/RenderOpenSearch"))
            //		&& (!conte|xt.Request.Path.Value.StartsWith("/AdminProfile"))
            //)
            {
                //var userIp = GetCurrentIpAddress(context);
                //var now = DateTime.Now;
                //var st = statisticsContext.Statistics.Where(c => c.IpAddress.Equals(userIp));
                //var temp = await st.FirstOrDefaultAsync(c => c.CreateDate.Year == now.Year && c.CreateDate.Month == now.Month && c.CreateDate.Day == now.Day);

                //if (temp != null)
                //{
                //	temp.Count++;
                //	temp.ModifyDate = DateTime.Now;
                //	statisticsContext.Entry(temp).Property(x => x.Count).IsModified = true;
                //	statisticsContext.Entry(temp).Property(x => x.ModifyDate).IsModified = true;
                //	//statisticsContext.Statistics.Update(temp);
                //	await statisticsContext.SaveChangesAsync();
                //}
                //else
                //{
                //	var request = context.Request;
                //	var ref1 = httpRequestInfo.GetReferrerUri();
                //	var referer = httpRequestInfo.GetReferrerUrl();
                //	var acLang = request.Headers["Accept-Language"];
                //	var header = request.Headers["User-Agent"];
                //	var userAgent = new UserAgent.UserAgent(header);

                //	//var routeValues = context.ActionDescriptor.RouteValues;
                //	//var controllerName = routeValues["Controller"];
                //	//var actionName = routeValues["Action"];
                //	//var pageName = routeValues["Page"];
                //	//var areaName = routeValues["Area"];

                //	var s = new Offishopp.Core.Domain.Entities.Statistics()
                //	{
                //		Count = 1,
                //		CreateDate = DateTime.Now,
                //		ModifyDate = DateTime.Now,
                //		Browser = userAgent.Browser.Name,
                //		Os = userAgent.Os.Name,
                //		UserAgent = header.ToString(),
                //		AcceptLanguage = acLang.ToString(),
                //		Host = request.Host.ToString(),
                //		IsHTTPS = request.IsHttps.ToString(),
                //		Method = request.Method,
                //		Path = request.Path,
                //		Protocol = request.Protocol,
                //		Scheme = request.Scheme,
                //		Referer = referer
                //	};
                //	if (context.User.Identity.IsAuthenticated)
                //	{
                //		s.UserId = context.User.GetUserId();
                //		s.IpAddress = userIp;
                //	}
                //	else
                //	{
                //		s.UserId = "NotSignIn";
                //		s.IpAddress = userIp;
                //	}
                //	await statisticsContext.Statistics.AddAsync(s);
                //	await statisticsContext.SaveChangesAsync();
                //}

                try
                {
                    //var hri = context;
                    //var s1 = hri.GetReferrerUri();
                    //var s12 = hri.GetReferrerUrl();
                    //var s121 = hri.GetRawUri();
                    //var s1211 = hri.GetRawUrl();
                    //var s12111 = hri.GetUserAgent();
                    //var s121111 = hri.GetUrlHelper();
                    //var s1211111 = hri.GetBaseUri();
                    //var s121111111 = hri.GetBaseUrl();
                    //var s1121111111 = hri.ReadRequestBodyAsStringAsync();
                    //var s11211111211 = hri.GetIP();

                    var referer = context.GetHeaderValue("Referer");
                    var userIp = GetCurrentIpAddress(context);
                    var request = context.Request;
                    var acLang = request.Headers["Accept-Language"];
                    var header = request.Headers["User-Agent"];
                    var userAgent = new UserAgent.UserAgent(header);

                    var s = new Demo.Statistics.Infrastructure.Entities.Statistics
                    {
                        Count = 1,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                        Browser = userAgent.Browser.Name,
                        Os = userAgent.Os.Name,
                        UserAgent = header.ToString(),
                        AcceptLanguage = acLang.ToString(),
                        Host = request.Host.ToString(),
                        IsHTTPS = request.IsHttps.ToString(),
                        Method = request.Method,
                        Path = request.Path,
                        Protocol = request.Protocol,
                        Scheme = request.Scheme,
                        Referer = referer
                    };
                    if (context.User.Identity.IsAuthenticated)
                    {
                        s.UserId = context.User.GetUserId();
                        s.IpAddress = userIp;
                    }
                    else
                    {
                        s.UserId = "NotSignIn";
                        s.IpAddress = userIp;
                    }
                    await statisticsContext.Statistics.AddAsync(s);
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
    }
}
