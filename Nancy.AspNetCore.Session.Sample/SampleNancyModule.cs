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
                //if session "sample" exist increment 
                if (int.TryParse(Session["sample"]?.ToString(),out int i))
                {
                    i++;
                    Session["sample"] = i;
                }
                else//otherwise initialise with 1
                {
                    Session["sample"] = 1;
                }
                var tc = new TestClass() { P = "abc" };
                Session["tc"] = tc;
                Session["sample2"] = "";
                Session["sample3"] = "";
                Session["sample4"] = "";

                foreach (var s in Session)
                {
                    string k = s.Key;
                    object v = s.Value;
                }

                var tc2 = Session["tc"];
                TestClass tc3 = Session["tc"] as TestClass;

                Session["sample5"] = null;
                //if null return empty string otherwise return "Hello [i]"
                return "Hello " + (Session["sample"]?.ToString() ?? "");
            });
        }
    }
}
