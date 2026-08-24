using Core;
using Core.Auth;
using Core.Swagger;
using UnidadesApi.Features.GetUnidadeById;
using UnidadesApi.Features.ListUnidades;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCore(builder.Configuration);
builder.Services.AddJwtBearerAuthentication(builder.Configuration);
builder.Services.AddSwaggerWithJwtAuth("Unidades API");

builder.Services.AddScoped<ListHandler>();
builder.Services.AddScoped<GetUnidadeByIdHandler>();

var app = builder.Build();

app.UseSwaggerWithUi();

app.UseAuthentication();
app.UseAuthorization();

app.MapListUnidadesEndpoint();
app.MapGetUnidadeByIdEndpoint();

app.Run();

public partial class Program
{
}
