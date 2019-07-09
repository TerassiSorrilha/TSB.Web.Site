using System;
using FluffySpoon.AspNet.LetsEncrypt;
using FluffySpoon.LetsEncrypt.Azure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TSB.Web.Site
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            //automatic renewal SSL Certificate on Let's Encrypt service.
            services.AddFluffySpoonLetsEncryptRenewalService(new LetsEncryptOptions(){
                Email = "crts.ms@outlook.com",
                UseStaging = false,
                Domains = new[] { Program.DomainToUse },
                TimeUntilExpiryBeforeRenewal = TimeSpan.FromDays(30),
                TimeAfterIssueDateBeforeRenewal = TimeSpan.FromDays(7),
                CertificateSigningRequest = new Certes.CsrInfo(){
                    CountryName = "Brazil",
                    Locality = "BR",
                    Organization = "TSBrothers",
                    OrganizationUnit = "Softwares",
                    State = "DF"
                }                
            });

            //get user running the service
            var managedIdentityCredentials = new AzureCredentialsFactory()
                .FromMSI(new MSILoginInformation(MSIResourceType.AppService), AzureEnvironment.AzureGlobalCloud);

            //set the persistence to azure storage
            services.AddFluffySpoonLetsEncryptAzureAppServiceSslBindingCertificatePersistence(
                new AzureOptions(){
                    ResourceGroupName = System.Environment.GetEnvironmentVariable("WEBSITE_RESOURCE_GROUP"),
                    Credentials = managedIdentityCredentials
                }
            );

            //persist the certificate to a file
            services.AddFluffySpoonLetsEncryptFileCertificatePersistence();            

            //persist challenges in-memory. challenges are the "/.well-known" URL codes that LetsEncrypt will call.
            services.AddFluffySpoonLetsEncryptMemoryChallengePersistence();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()){
                app.UseDeveloperExceptionPage();
            }
            else{
                app.UseHsts();
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseFluffySpoonLetsEncryptChallengeApprovalMiddleware();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
