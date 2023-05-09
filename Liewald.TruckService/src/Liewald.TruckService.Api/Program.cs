using System.Security.Claims;
using Liewald.TruckService.Application;
using Liewald.TruckService.Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IServiceCollection services = builder.Services;

services
    .AddOptions<CosmosDatabaseOptions>()
    .BindConfiguration(CosmosDatabaseOptions.ConfigurationSectionKey)
    .ValidateDataAnnotations()
    .ValidateOnStart();

services
    .AddTruckServiceApplication(builder.Configuration)
    .AddLogging();

services
    .AddAuthentication(config => config.DefaultScheme = JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Audience = builder.Configuration["ClientID"];
        options.Authority = builder.Configuration["AzureAD:Authority"];
    });

services
    .AddAuthorization(options =>
    {
        options.AddPolicy("MustBeHeinz", policyBuilder =>
            policyBuilder
                .RequireAuthenticatedUser()
                .RequireAssertion(context => context.User.HasClaim(ClaimTypes.GivenName, "Heinz"))
                .Build());

        options.AddPolicy("MustBeJohn", policyBuilder =>
            policyBuilder
                .RequireAuthenticatedUser()
                .RequireAssertion(context => context.User.HasClaim(ClaimTypes.GivenName, "John"))
                .Build());
    });

services.AddOpenApiDocument(config =>
{
    config.PostProcess = document => document.Info.Title = "TruckService";

    config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("bearer"));

    config.AddSecurity("bearer", Enumerable.Empty<string>(), new OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.OAuth2,
        Description = "Azure AD authentication",
        Flow = OpenApiOAuth2Flow.Implicit,
        Flows = new OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow()
            {
                Scopes = GetScopes(builder.Configuration),
                AuthorizationUrl = builder.Configuration["ApiDoc:AuthorizationUrl"],
                TokenUrl = builder.Configuration["ApiDoc:TokenUrl"],
            },
        },
    });
});

services.AddControllers();

WebApplication app = builder.Build();

#if DEBUG
using (IServiceScope scope = app.Services.CreateScope())
{
    await scope.CreateContainersAsync().ConfigureAwait(false);
}
#endif

app
    .UseHttpsRedirection()
    .UseRouting()
    .UseAuthentication()
    .UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app
        .UseOpenApi()
        .UseSwaggerUi3(
            settings =>
            {
                settings.DocExpansion = "list";
                settings.OAuth2Client = new OAuth2ClientSettings
                {
                    ClientId = builder.Configuration["ClientID"],
                    AppName = "TruckService",
                };
                settings.OAuth2Client.Scopes.Add(builder.Configuration["ApiDoc:Scope"]);
            });
}

app.MapControllers();

await app.RunAsync().ConfigureAwait(false);

static Dictionary<string, string> GetScopes(ConfigurationManager configuration)
{
    string? scope = configuration["ApiDoc:Scope"];

    if (string.IsNullOrWhiteSpace(scope))
    {
        throw new ArgumentNullException(nameof(scope));
    }

    return new()
    {
        {
            scope, string.Empty
        },
    };
}