using FluffySpoon.AspNet.LetsEncrypt;
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
                .UseKestrel(kestrelOptions  => kestrelOptions.ConfigureHttpsDefaults(
                    httpsOptions => httpsOptions.ServerCertificateSelector = (c, s) => LetsEncryptRenewalService.Certificate))
                .UseUrls("http://" + DomainToUse, "https://" + DomainToUse)
                .UseStartup<Startup>();
    }
}
