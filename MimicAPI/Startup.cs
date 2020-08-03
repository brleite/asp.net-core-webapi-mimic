using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MimicAPI.Database;
using MimicAPI.V1.Repositories.Contracts;
using MimicAPI.V1.Repositories;
using AutoMapper;
using MimicAPI.Helpers;
using Microsoft.AspNetCore.Mvc.Versioning;
using MimicAPI.Helpers.Swagger;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;

namespace MimicAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public Profile DTOMapperProfile { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            #region AutoMapper-Config
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DTOMapperProfile());
            });
            IMapper mapper = config.CreateMapper();

            services.AddSingleton(mapper);
            #endregion

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            /*
            services.AddMvc(options =>
            {
                options.Filters.Add(new ConsumesAttribute("application/json"));
                options.Filters.Add(new ProducesAttribute("application/json"));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            */

            services.AddDbContext<MimicContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("MimicContext"), builder =>
                    builder.MigrationsAssembly("MimicAPI")));

            services.AddScoped<IPalavraRepository, PalavraRepository>();
            services.AddApiVersioning(cfg =>
            {
                cfg.ReportApiVersions = true;
                // cfg.ApiVersionReader = new HeaderApiVersionReader("api-version");
                cfg.AssumeDefaultVersionWhenUnspecified = true;
                cfg.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
            });

            services.AddSwaggerGen(cfg =>
            {
                // Resolve conflito de controllers com a mesma rota e mesmo nome. Comum no caso de versionamento de APIs
                cfg.ResolveConflictingActions(apiDescription => apiDescription.First());
                cfg.SwaggerDoc("v2.0", new Info()
                {
                    Title = "MimicAPI - V2.0",
                    Version = "v2.0"
                });
                cfg.SwaggerDoc("v1.1", new Info()
                {
                    Title = "MimicAPI - V1.1",
                    Version = "v1.1"
                });
                cfg.SwaggerDoc("v1.0", new Info()
                {
                    Title = "MimicAPI - V1.0",
                    Version = "v1.0"
                });

                var caminhoProjeto = PlatformServices.Default.Application.ApplicationBasePath;
                var nomeProjeto = $"{PlatformServices.Default.Application.ApplicationName}.xml";
                var caminhoArquivoXmlComentario = Path.Combine(caminhoProjeto, nomeProjeto);

                cfg.IncludeXmlComments(caminhoArquivoXmlComentario);
                cfg.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var actionApiVersionModel = apiDesc.ActionDescriptor?.GetApiVersion();
                    // would mean this action is unversioned and should be included everywhere
                    if (actionApiVersionModel == null)
                    {
                        return true;
                    }
                    if (actionApiVersionModel.DeclaredApiVersions.Any())
                    {
                        return actionApiVersionModel.DeclaredApiVersions.Any(v => $"v{v.ToString()}" == docName);
                    }
                    return actionApiVersionModel.ImplementedApiVersions.Any(v => $"v{v.ToString()}" == docName);
                });
                cfg.OperationFilter<ApiVersionOperationFilter>();

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            // Inclui mensagem no retorno da requisição
            app.UseStatusCodePages();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseSwagger(); //swagger/v1/swagger.json - Parâmetro RouteTemplate
            app.UseSwaggerUI(cfg =>
            {
                cfg.SwaggerEndpoint("/swagger/v2.0/swagger.json", "MimicAPI v2.0");
                cfg.SwaggerEndpoint("/swagger/v1.1/swagger.json", "MimicAPI v1.1");
                cfg.SwaggerEndpoint("/swagger/v1.0/swagger.json", "MimicAPI v1.0");
                // cfg.RoutePrefix = String.Empty; // Ao acessar o servidor, já é encaminhado automaticamente para o swagger
            });
        }
    }
}
