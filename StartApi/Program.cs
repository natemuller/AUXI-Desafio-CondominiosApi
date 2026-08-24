using AutenticacaoApi.Features.Login;
using BlocosApi.Features.GetBlocoByChave;
using BlocosApi.Features.ListBlocos;
using CondominiosApi.Features.GetByCodCondom;
using CondominiosApi.Features.ListCondominios;
using Core;
using Core.Auth;
using Core.Swagger;
using UnidadesApi.Features.GetUnidadeById;
using UnidadesApi.Features.ListUnidades;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCore(builder.Configuration);
builder.Services.AddJwtBearerAuthentication(builder.Configuration);
builder.Services.AddSwaggerWithJwtAuth("StartApi");

builder.Services.Configure<DevCredentialOptions>(
    builder.Configuration.GetSection("DevCredential"));

// "ListHandler" existe em CondominiosApi/BlocosApi/UnidadesApi com o mesmo
// nome simples (namespaces diferentes) — precisa qualificar para não ambiguar
// aqui, onde os 3 namespaces estão em uso ao mesmo tempo.
builder.Services.AddScoped<LoginHandler>();
builder.Services.AddScoped<CondominiosApi.Features.ListCondominios.ListHandler>();
builder.Services.AddScoped<GetByCodCondomHandler>();
builder.Services.AddScoped<BlocosApi.Features.ListBlocos.ListHandler>();
builder.Services.AddScoped<GetBlocoByChaveHandler>();
builder.Services.AddScoped<UnidadesApi.Features.ListUnidades.ListHandler>();
builder.Services.AddScoped<GetUnidadeByIdHandler>();

var app = builder.Build();

app.UseSwaggerWithUi();

app.UseAuthentication();
app.UseAuthorization();

app.MapLoginEndpoint();
app.MapListCondominiosEndpoint();
app.MapGetByCodCondomEndpoint();
app.MapListBlocosEndpoint();
app.MapGetBlocoByChaveEndpoint();
app.MapListUnidadesEndpoint();
app.MapGetUnidadeByIdEndpoint();

app.Run();

public partial class Program
{
}
