using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Nancy.AspNetCore.Session;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nancy.Owin
{
    public static class NancyAspNetCoreSessionExtension
    {
        public static IServiceCollection AddNancyAspnetCoreSession(this IServiceCollection services)
        {
            services.AddSingleton(InternalHttpContextAccessorSingleton.Instance.HttpContextAccessor);
            return services;
        }
    }
}
