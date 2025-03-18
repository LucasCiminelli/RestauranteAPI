using Microsoft.OpenApi.Models;
using Restaurants.API.Middlewares;
using Serilog;

namespace Restaurants.API.Extensions
{
    public static class WebApplicationBuilderExtensions
    {

        public static void AddPresentation(this WebApplicationBuilder builder)
        {

            builder.Services.AddAuthentication(); //get the token and use this token in the form of header as a bearer token for any Http Request.

            builder.Services.AddControllers();

            builder.Services.AddSwaggerGen(config
                =>
            {
                config.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });

                config.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference{ Type = ReferenceType.SecurityScheme, Id = "bearerAuth"}
            },
            []
        }
    });

            });

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddScoped<ErrorHandlingMiddleware>(); //adding error middleware
            builder.Services.AddScoped<RequestTimeLoggingMiddleware>();

            //serilog
            builder.Host.UseSerilog((context, configuration) =>
            {
                configuration
                .ReadFrom.Configuration(context.Configuration);
                //refactoring this manual configuration to the appsetings.json.
                //.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                //.MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
                //.WriteTo.File("Logs/Restaurant-API-.log", rollingInterval:RollingInterval.Day, rollOnFileSizeLimit:true)
                //.WriteTo.Console(outputTemplate: "[{Timestamp:dd-MM HH:mm:ss} {Level:u3}] |{SourceContext}| {NewLine} {Message:lj}{NewLine}{Exception}");
            });
        }
    }
}
