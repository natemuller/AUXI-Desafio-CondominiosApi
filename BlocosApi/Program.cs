using BlocosApi.Features.GetBlocoByChave;
using BlocosApi.Features.ListBlocos;
using Core;
using Core.Auth;
using Core.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCore(builder.Configuration);
builder.Services.AddJwtBearerAuthentication(builder.Configuration);
builder.Services.AddSwaggerWithJwtAuth("Blocos API");

builder.Services.AddScoped<ListHandler>();
builder.Services.AddScoped<GetBlocoByChaveHandler>();

var app = builder.Build();

app.UseSwaggerWithUi();

app.UseAuthentication();
app.UseAuthorization();

app.MapListBlocosEndpoint();
app.MapGetBlocoByChaveEndpoint();

app.Run();

public partial class Program
{
}
