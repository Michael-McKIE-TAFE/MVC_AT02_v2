using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace McKIESales.API {
    internal class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions> {
        private readonly IApiVersionDescriptionProvider _provider;
        private readonly IOptions<ApiVersioningOptions> _options;

        public ConfigureSwaggerOptions (IApiVersionDescriptionProvider provider, IOptions<ApiVersioningOptions> options){
            _provider = provider;
            _options = options;
        }

        //  This function configures Swagger documentation for each API version by including XML
        //  comments (if available) and adding a corresponding Swagger document using version-specific
        //  metadata. It loops through all API version descriptions provided by `_provider`.
        public void Configure (SwaggerGenOptions options){
            foreach (var description in _provider.ApiVersionDescriptions){
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                if (File.Exists(xmlPath)){
                    options.IncludeXmlComments(xmlPath);
                }

                options.SwaggerDoc(description.GroupName, CreateVersionInfo(description));
            }
        }

        //  This function generates an `OpenApiInfo` object for a given API version,
        //  setting its title and version number. It appends notes to the description if
        //  the version is the default or deprecated, helping document consumers understand
        //  the status of each API version.
        private OpenApiInfo CreateVersionInfo (ApiVersionDescription description){
            var info = new OpenApiInfo(){ 
                Title = $"McKIESales.API {description.GroupName}",
                Version = description.ApiVersion.ToString()
            };

            if (description.ApiVersion == _options.Value.DefaultApiVersion){
                if (!string.IsNullOrWhiteSpace(info.Description)){
                    info.Description += " ";
                }

                info.Description += "Default API version.";
            }

            if (description.IsDeprecated){
                if (!string.IsNullOrWhiteSpace(info.Description)){
                    info.Description += " ";
                }

                info.Description += "This API version is deprecated.";
            }

            return info;
        }
    }
}