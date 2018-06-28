using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ProjectOxford.Face;
using ProfileManager.AppService;
using ProfileManager.Entities;

namespace ProfileManager.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddAzureAd(options => Configuration.Bind("AzureAd", options))
            .AddCookie();

            // todo: refactor configuration; let callers configure via ioptions 
            services.Configure<DocumentProviderOptions>(Configuration.GetSection("DocumentProvider"));
            services.Configure<FaceInfoProviderOptions>(Configuration.GetSection("FaceInfoProvider"));

            // todo: refactor, these probably don't need to be singleton but need to consider implications before switching to transient, especially with httpclient
            services.AddSingleton<IDocumentProvider<Employee>, CosmosDocumentProvider<Employee>>();
            services.AddSingleton<IEmployeeRepository, DocumentEmployeeRepository>();
            services.AddSingleton<IFaceServiceClient, FaceServiceClient>(x =>
            {
                return new FaceServiceClient(Configuration["FaceInfoProvider:Key"], Configuration["FaceInfoProvider:Endpoint"]);
            });
            services.AddSingleton<IFaceInfoProvider, AzureOxfordFaceInfoProvider>();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Employee}/{action=List}/{id?}");
            });
        }
    }
}
