using Demo.Data;
using Demo.IntegratingGoogleAnalytics;
using Demo.SegmentClientSideAnalytics;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

            #region Server-Side Analytics

            #endregion

            services.AddControllersWithViews();
            services.AddRazorPages();
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
            app.UseHttpsRedirection();

            app.UseAnalytics();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseAnalytics("UA-XXXX-Y");
            app.UseAnalytics(trackers =>
            {
                trackers.AddTracker("UA-XXXX-Y");
            });

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
