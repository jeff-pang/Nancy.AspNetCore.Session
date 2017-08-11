using Nancy;

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
                
                //if null return empty string otherwise return "Hello [i]"
                return "Hello " + (Session["sample"]?.ToString() ?? "");
            });
        }
    }
}
