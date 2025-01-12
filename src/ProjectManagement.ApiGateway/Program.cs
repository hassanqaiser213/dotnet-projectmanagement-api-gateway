using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ocelot.Authorization;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;

namespace ProjectManagement.ApiGateway;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
            .AddOcelot("Ocelot", builder.Environment)
            .AddEnvironmentVariables();

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer("Auth0", options =>
            {
                options.Authority = "https://afrozeprojectmanagement.us.auth0.com/";
                options.Audience = "company";
            });

        builder.Services
            .AddOcelot()
            .AddConsul();
        
        builder.Services.CustomizeOcelot();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy => { policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
        });
        
        WebApplication app = builder.Build();
        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseOcelot().Wait();

        app.Run();
    }
}

// Github Issue: https://github.com/ThreeMammals/Ocelot/issues/913
internal static class DependencyInjectionExtensions
{
    internal static void CustomizeOcelot(this IServiceCollection services)
    {
        services.RemoveAll<IScopesAuthorizer>();
        services.TryAddSingleton<IScopesAuthorizer, DelimitedScopesAuthorizer>();
    }
}