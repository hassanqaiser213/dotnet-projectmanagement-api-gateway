
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
        builder.Configuration.AddJsonFile("ocelot.json");

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

        WebApplication app = builder.Build();
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