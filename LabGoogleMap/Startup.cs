using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LabGoogleMap
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddTransient<MapService, MapService>();
            services.AddTransient<LabContext, LabContext>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            //app.UseMvcWithDefaultRoute();
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");






            });

            //            routes.MapRouteLowerCase(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "dashboard", action = "index", id = UrlParameter.Optional }
            //);    

            //// Attribute routing
            ///
            //var routeOptions = new RouteOptions();
            //routeOptions.ConstraintMap.Add("hasroutevalue", typeof(RouteValuePresentConstraint));
            //routeOptions.ConstraintMap.Add("values", typeof(ValuesConstraint));

            //var constraintsResolver = new DefaultInlineConstraintResolver(
            //    routeOptions
            //    );

            //routes.MapMvcAttributeRoutes(constraintsResolver);


            //var constraintsResolver = new DefaultInlineConstraintResolver();
            //constraintsResolver.ConstraintMap.Add("hasroutevalue", typeof(RouteValuePresentConstraint));
            //constraintsResolver.ConstraintMap.Add("values", typeof(ValuesConstraint));
            //routes.MapMvcAttributeRoutes(constraintsResolver);

        }
    }


    public class RouteValuePresentConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (values.ContainsKey(routeKey))
            {
                return true;
            }
            return false;
        }
    }

    public class ValuesConstraint : IRouteConstraint
    {
        private readonly string[] validOptions;
        public ValuesConstraint(string options)
        {
            validOptions = options.Split('|');
        }

        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            object value;
            if (values.TryGetValue(routeKey, out value) && value != null)
            {
                return validOptions.Contains(value.ToString(), StringComparer.OrdinalIgnoreCase);
            }
            return false;
        }
    }

}
