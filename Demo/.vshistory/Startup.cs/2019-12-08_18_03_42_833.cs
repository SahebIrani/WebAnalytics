using System.Threading.Tasks;

using Demo.Data;
using Demo.IntegratingGoogleAnalytics;
using Demo.SegmentClientSideAnalytics;
using Demo.ServerSideAnalytics;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using ServerSideAnalytics;
using ServerSideAnalytics.SqlServer;

using Wangkanai.Analytics.Core.Builder;

namespace Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            #region Wangkanai.Analytics
            // Add application services.
            services.AddAnalytics(options => { });
            services.AddAnalytics()
                    .AddDisplayFeatures()
                    .AddLinkAttribution()
                    .AddLinker()
            ;
            services.AddAnalytics().AddEcommerce(); //EcommerceSandbox
            services.AddAnalytics().AddEcommerceEnhanced(); //EcommerceEnhanced
            #endregion

            #region Integrating Google Analytics
            // Register the Google Analytics configuration
            services.Configure<GoogleAnalyticsOptions>(options => Configuration.GetSection("GoogleAnalytics").Bind(options));

            // Register the TagHelperComponent
            services.AddTransient<ITagHelperComponent, GoogleAnalyticsTagHelperComponent>();
            #endregion

            #region Segment Client-Side Analytics
            services.Configure<SegmentSettings>(Configuration.GetSection("Segment"));
            #endregion

            #region ServerSideAnalytics

            //SqlServer

            //Sqlite
            services.AddTransient<IAnalyticStore>(_ => GetAnalyticStore());

            //Mongo

            #endregion

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        private IAnalyticStore GetAnalyticStore()
        {
            return new SqLiteAnalyticStore("Data Source = stat.db");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            #region Server-Side Analytics

            // get real IP from reverse proxy
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            // rewrite some headers for more security
            app.Use(async (context, next) =>
            {
                context.Response.OnStarting(() =>
                {
                    // if Antiforgery hasn't already set this header
                    if (string.IsNullOrEmpty(context.Response.Headers["X-Frame-Options"]))
                    {
                        // do not allow to put your website pages into frames (prevents clickjacking)
                        context.Response.Headers.Add("X-Frame-Options", "DENY");
                    }
                    // check MIME types (prevents MIME-based attacks)
                    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                    // hide server information
                    context.Response.Headers.Add("Server", "ololo");
                    // allow to load scripts only from listed sources
                    //context.Response.Headers.Add("Content-Security-Policy", "default-src 'self' *.google-analytics.com; style-src 'self' 'unsafe-inline'; script-src 'self' 'unsafe-inline'");
                    return Task.FromResult(0);
                });

                await next();
            });

            // our custom analytics middleware
            app.UseAnalytics();

            #endregion

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            #region Wangkanai.Analytics
            app.UseAnalytics("UA-XXXX-Y");
            app.UseAnalytics(trackers =>
            {
                trackers.AddTracker("UA-XXXX-Y");
            });
            #endregion

            #region ServerSideAnalytics

            //SqlServer
            app.UseServerSideAnalytics(new SqlServerAnalyticStore
                (
                    "Server=wczmf185129-u54;Database=test;Trusted_Connection=True;"
                )
            );

            //Sqlite
            app.UseServerSideAnalytics(GetAnalyticStore());

            //Mongo

            #endregion

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
