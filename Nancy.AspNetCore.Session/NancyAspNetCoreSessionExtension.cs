using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Nancy.AspNetCore.Http;
using Nancy.AspNetCore.Http.Accessors;
using Nancy.AspNetCore.Session;
using Nancy.Bootstrapper;
using System;

namespace Nancy.Owin
{
    public static class NancyAspNetCoreSessionExtension
    {
        public static IHttpContextAccessor httpCtxAcs = null;
        private static bool isSessionEnabled = false;
        public static IServiceCollection AddNancyAspnetCoreSession(this IServiceCollection service)
        {
            httpCtxAcs = Factory.Create(service);
            return service;
        }

        public static IApplicationBuilder UseNancyAspnetCoreSession(this IApplicationBuilder builder)
        {
            if (!isSessionEnabled)
            {
                INancyAccessors nancyAccessors = Factory.Create(builder);
                IPipelines piplines = nancyAccessors.GetPipelines();

                if (httpCtxAcs == null)
                    throw new InvalidOperationException("AddNancyAspnetCoreSession is not initialised");

                NancyAspNetCoreSession.Enable(piplines, httpCtxAcs);
                isSessionEnabled = true;
            }

            return builder;
        }
    }
}
