using System;
using System.Collections.Generic;
using System.Text;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace Nancy.AspNetCore.Session.Sample
{
    public class SampleNancyBootstrapper:DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            //enable AspNetCore Session in Nancy
            NancyAspNetCoreSession.Enable(pipelines);
            base.ApplicationStartup(container, pipelines);
        }
    }
}