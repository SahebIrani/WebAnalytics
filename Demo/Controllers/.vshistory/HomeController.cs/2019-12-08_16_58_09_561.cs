using System.Diagnostics;

using Demo.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Demo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) => _logger = logger;

        public IActionResult Index() => View();






        //async Task<SomeKindOfDataStructure[]> GetUsageFromGoogleAnalytics(DateTime startAtThisDate, DateTime endAtThisDate)
        //{
        //    // Create the DateRange object. Here we want data from last week.
        //    var dateRange = new DateRange
        //    {
        //        StartDate = startAtThisDate.ToString("yyyy-MM-dd"),
        //        EndDate = endAtThisDate.ToString("yyyy-MM-dd")
        //    };
        //    // Create the Metrics and dimensions object.
        //    // var metrics = new List<Metric> { new Metric { Expression = "ga:sessions", Alias = "Sessions" } };
        //    // var dimensions = new List<Dimension> { new Dimension { Name = "ga:pageTitle" } };
        //    var metrics = new List<Metric> { new Metric { Expression = "ga:uniquePageviews" } };
        //    var dimensions = new List<Dimension> {
        //        new Dimension { Name = "ga:date" },
        //        new Dimension { Name = "ga:dimension1" }
        //    };

        //    // Get required View Id from configuration
        //    var viewId = $"ga:{"[VIEWID]"}";

        //    // Create the Request object.
        //    var reportRequest = new ReportRequest
        //    {
        //        DateRanges = new List<DateRange> { dateRange },
        //        Metrics = metrics,
        //        Dimensions = dimensions,
        //        FiltersExpression = "ga:pagePath==/index.html",
        //        ViewId = viewId
        //    };

        //    var getReportsRequest = new GetReportsRequest
        //    {
        //        ReportRequests = new List<ReportRequest> { reportRequest }
        //    };

        //    //Invoke Google Analytics API call and get report
        //    var analyticsService = GetAnalyticsReportingServiceInstance();
        //    var response = await (analyticsService.Reports.BatchGet(getReportsRequest)).ExecuteAsync();

        //    var logins = response.Reports[0].Data.Rows.Select(row => new SomeKindOfDataStructure
        //    {
        //        Date = new DateTime(
        //            year: Convert.ToInt32(row.Dimensions[0].Substring(0, 4)),
        //            month: Convert.ToInt32(row.Dimensions[0].Substring(4, 2)),
        //            day: Convert.ToInt32(row.Dimensions[0].Substring(6, 2))),
        //        NumberOfLogins = Convert.ToInt32(row.Metrics[0].Values[0])
        //    })
        //    .OrderByDescending(login => login.Date)
        //    .ToArray();

        //    return logins;
        //}

        ///// <summary>
        ///// Intializes and returns Analytics Reporting Service Instance
        ///// </summary>
        //AnalyticsReportingService GetAnalyticsReportingServiceInstance()
        //{
        //    var googleAuthFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        //    {
        //        ClientSecrets = new ClientSecrets
        //        {
        //            ClientId = "[CLIENTID]",
        //            ClientSecret = "[CLIENTSECRET]"
        //        }
        //    });

        //    var responseToken = new TokenResponse
        //    {
        //        AccessToken = "[ANALYTICSTOKEN]",
        //        RefreshToken = "[REFRESHTOKEN]",
        //        Scope = AnalyticsReportingService.Scope.AnalyticsReadonly, //Read-only access to Google Analytics,
        //        TokenType = "Bearer",
        //    };

        //    var credential = new UserCredential(googleAuthFlow, "", responseToken);

        //    // Create the  Analytics service.
        //    return new AnalyticsReportingService(new BaseClientService.Initializer
        //    {
        //        HttpClientInitializer = credential,
        //        ApplicationName = "my-super-applicatio",
        //    });
        //}

















        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
