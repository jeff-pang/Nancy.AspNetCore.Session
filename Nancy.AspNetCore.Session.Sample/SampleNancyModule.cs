using System;
using System.Collections.Generic;
using System.Text;

namespace Nancy.AspNetCore.Session.Sample
{
    public class SampleNancyModule:NancyModule
    {
        public SampleNancyModule()
        {
            Get("/", p =>
            {
                if (int.TryParse(Session["sample"]?.ToString(),out int i))
                {
                    i++;
                    Session["sample"] = i;
                }
                else
                {
                    Session["sample"] = 1;
                }

                return "Hello " + (Session["sample"]?.ToString() ?? "");
            });
        }
    }
}
