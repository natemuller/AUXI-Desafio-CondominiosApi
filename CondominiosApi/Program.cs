using CondominiosApi.Features.GetByCodCondom;
using CondominiosApi.Features.ListCondominios;
using Core;
using Core.Auth;
using Core.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCore(builder.Configuration);
builder.Services.AddJwtBearerAuthentication(builder.Configuration);
builder.Services.AddSwaggerWithJwtAuth("Condominios API");

builder.Services.AddScoped<ListHandler>();
builder.Services.AddScoped<GetByCodCondomHandler>();

var app = builder.Build();

app.UseSwaggerWithUi();

app.UseAuthentication();
app.UseAuthorization();

app.MapListCondominiosEndpoint();
app.MapGetByCodCondomEndpoint();

app.Run();

public partial class Program
{
}
