using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace TSB.Web.Site
{
    public class Program
    {
        public const string DomainToUse = "tsbrothers.com.br";
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://" + DomainToUse, "https://" + DomainToUse)
                .UseStartup<Startup>();
    }
}
