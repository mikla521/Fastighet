using Microsoft.Identity.Web;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

#region Add services to the container
builder.Services
.AddAuthentication("Bearer")
.AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddControllers();

builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new();

        document.Components.SecuritySchemes ??=
            new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes["oauth2"] =
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl =
                            new Uri(
                                $"https://login.microsoftonline.com/{builder.Configuration["Entra:TenantId"]}/oauth2/v2.0/authorize"),

                        TokenUrl =
                            new Uri(
                                $"https://login.microsoftonline.com/{builder.Configuration["Entra:TenantId"]}/oauth2/v2.0/token"),

                        Scopes = new Dictionary<string, string>
                        {
                            [
                                $"api://{builder.Configuration["Entra:ApiClientId"]}/access_as_user"
                            ] = "Access Fastighetsskötsel API"
                        }
                    }
                }
            };

        return Task.CompletedTask;
    });

    options.AddOperationTransformer((operation, context, cancellationToken) =>
    {
        operation.Security ??= [];

        operation.Security.Add(
            new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("oauth2")] = []
            });

        return Task.CompletedTask;
    });
}); 
#endregion

var app = builder.Build();

#region Configure OpenApi - Scalar
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.Authentication = new ScalarAuthenticationOptions
        {
            PreferredSecuritySchemes = ["oauth2"],

            SecuritySchemes = new Dictionary<string, ScalarSecurityScheme>
            {
                ["oauth2"] = new ScalarOAuth2SecurityScheme
                {
                    DefaultScopes =
                    [
                        $"api://{builder.Configuration["Entra:ApiClientId"]}/access_as_user"
                    ],

                    Flows = new ScalarFlows
                    {
                        AuthorizationCode = new AuthorizationCodeFlow
                        {
                            ClientId = builder.Configuration["Entra:TestClientId"]
                        }
                    }
                }
            }
        };
    });
}
#endregion

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
