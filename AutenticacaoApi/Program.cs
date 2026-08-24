using AutenticacaoApi.Features.Login;
using Core;
using Core.Auth;
using Core.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCore(builder.Configuration);
builder.Services.AddJwtBearerAuthentication(builder.Configuration);
builder.Services.AddSwaggerWithJwtAuth("Autenticacao API");

builder.Services.Configure<DevCredentialOptions>(
    builder.Configuration.GetSection("DevCredential"));

builder.Services.AddScoped<LoginHandler>();

var app = builder.Build();

app.UseSwaggerWithUi();

app.UseAuthentication();
app.UseAuthorization();

app.MapLoginEndpoint();

app.Run();

public partial class Program
{
}
