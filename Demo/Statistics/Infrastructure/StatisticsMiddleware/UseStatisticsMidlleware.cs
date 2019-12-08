using Microsoft.AspNetCore.Builder;

namespace Demo.Statistics.Infrastructure.StatisticsMiddleware
{
    public static class UseStatisticsMidlleware
    {
        public static void UseStatistics(this IApplicationBuilder applicationBuilder) =>
            applicationBuilder.UseMiddleware<StatisticsMiddleware>();
    }
}
