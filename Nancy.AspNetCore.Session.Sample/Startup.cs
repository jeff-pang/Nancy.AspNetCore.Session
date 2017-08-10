using System;
using Microsoft.Extensions.DependencyInjection;
using Nancy.Owin;
using Microsoft.AspNetCore.Builder;

namespace Nancy.AspNetCore.Session.Sample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDistributedMemoryCache()
                .AddSession()
                .AddNancyAspnetCoreSession();
        }

        public void Configure(IApplicationBuilder app)
        {
            app
            .UseSession()
            .UseOwin(x => {
                x.UseNancy();
            });
        }
    }
}
