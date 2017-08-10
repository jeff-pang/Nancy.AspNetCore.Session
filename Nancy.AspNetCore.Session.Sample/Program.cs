using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Nancy.AspNetCore.Session.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}