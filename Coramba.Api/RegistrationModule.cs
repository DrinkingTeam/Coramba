using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Coramba.Api.Controllers;
using Coramba.DependencyInjection.Annotations;
using Coramba.DependencyInjection.Modules;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Coramba.Api
{
    [AutoModule(DependsOn = new[]
    {
        typeof(Services.RegistrationModule)
    })]
    public sealed class RegistrationModule : ModuleBuilder<RegistrationModule, RegistrationModule.ComponentInfo>
    {
        public class ComponentInfo
        {
            public string Title { get; set; }

            public bool EnableSwaggerGen { get; set; }
            public string[] XmlDocFileNames { get; set; }
            public Action<SwaggerGenOptions> SwaggerGenBuilder { get; set; }

            public bool EnableApiVersioning { get; set; }
            public bool EnableVersionedApiExplorer { get; set; }
        }

        public RegistrationModule Title(string title)
            => WithComponent(c => c.Title = title);

        public RegistrationModule SwaggerGen(Action<SwaggerGenOptions> swaggerGenBuilder)
            => WithComponent(c =>
            {
                c.EnableSwaggerGen = true;
                c.SwaggerGenBuilder = swaggerGenBuilder;
            });

        public RegistrationModule ApiVersioning()
            => WithComponent(c => c.EnableApiVersioning = true);

        public RegistrationModule VersionedApiExplorer()
            => WithComponent(c => c.EnableVersionedApiExplorer = true);

        public RegistrationModule XmlDocFiles(params string[] xmlDocFileNames)
            => WithComponent(c => c.XmlDocFileNames = xmlDocFileNames);

        public RegistrationModule XmlDocFiles(params Assembly[] xmlDocAssemblies)
            => XmlDocFiles(xmlDocAssemblies.Select(a => Path.ChangeExtension(a.Location, ".xml")).ToArray());

        protected override void Register()
        {
            if (Component.EnableSwaggerGen)
            {
                Services.AddSwaggerGen(swagger =>
                {
                    swagger.OperationFilter<SwaggerDefaultValues>();

                    foreach (var xmlDocFileName in Component.XmlDocFileNames ?? Enumerable.Empty<string>())
                    {
                        if (!string.IsNullOrWhiteSpace(xmlDocFileName))
                        {
                            if (File.Exists(xmlDocFileName))
                                swagger.IncludeXmlComments(xmlDocFileName);
                        }
                    }

                    swagger.EnableAnnotations();

                    Component.SwaggerGenBuilder?.Invoke(swagger);
                });
            }

            if (Component.EnableApiVersioning)
            {
                Services.AddApiVersioning(cfg =>
                {
                    cfg.ReportApiVersions = true;
                });
            }

            if (Component.EnableVersionedApiExplorer)
            {
                Services.AddVersionedApiExplorer(cfg =>
                {
                    cfg.GroupNameFormat = "'v'VVVV";
                    cfg.SubstituteApiVersionInUrl = true;
                });
            }

            if (Component.EnableSwaggerGen && Component.EnableApiVersioning && Component.EnableVersionedApiExplorer)
            {
                Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>(sp =>
                {
                    return ActivatorUtilities.CreateInstance<ConfigureSwaggerOptions>(sp, Component.Title ?? "Api");
                });
            }
        }

        protected override void RegisterOnce()
        {
            Services.TryAddScoped<ApiBaseControllerContext>();
            Services.TryAddScoped<ModelValidationActionFilter>();
        }
    }
}
