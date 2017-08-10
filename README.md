# Nancy.AspNetCore.Session [![Build status](https://ci.appveyor.com/api/projects/status/iu1rdtei2wcn5rfq?svg=true)](https://ci.appveyor.com/project/jeff-pang/nancy-aspnetcore-session)
Enables AspNetCore Sessions for use in Nancy

This is a lightweight middleware that integrates Nancy Sessions with AspNetCore Sessions, and then you can get use any other AspNetCore's session middleware in Nancy.

To enable this first in your `Startup` class's `ConfigureServices` do the following.

### Install

Nancy AspNetCore Session is available on NuGet:

```
Install-Package Nancy.AspNetCore.Session
```

#### Step 1.

Call `services.AddNancyAspnetCoreSession();` e.g:

```C#
public void ConfigureServices(IServiceCollection services)
{
    services
        //Add in-proc memory Session Cache Middleware, 
        //this can be any other middleware such as Redis Cache
        .AddDistributedMemoryCache()
        //Add AspNetCore Session
        .AddSession()
        //Wire up AspNetCore with Nancy
        .AddNancyAspnetCoreSession(); 
}
```

#### Step 2.

Create a custom nancy bootstrapper by inheriting the `DefaultNancyBootstrapper`. Then enable the session.

```C#
public class SampleNancyBootstrapper:DefaultNancyBootstrapper
{
    protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
    {
        NancyAspNetCoreSession.Enable(pipelines);
        base.ApplicationStartup(container, pipelines);
    }
}
```

And that's it. Nancy session is now wired up to AspNetCore session and you can access it using Nancy's Session like this:

```C#
public class SampleNancyModule:NancyModule
{
    public SampleNancyModule()
    {
        Get("/{myname}", p =>
        {
            Session["sample"] = (string)p.myname;

            return "Hello " + (Session["sample"]?.ToString() ?? "");
        });
    }
}
```
