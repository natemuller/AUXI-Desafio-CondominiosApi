using System.Text.Json;
using CondominiosApi.Tests.TestDoubles;
using Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CondominiosApi.Tests.Caching;

public class ResponseCacheEndpointFilterTests
{
    private static DefaultHttpContext CriarHttpContext(
        FakeCacheService cacheService,
        string path = "/api/condominios",
        string queryString = "")
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<ICacheService>(cacheService);
        services.AddSingleton<IOptions<JsonOptions>>(Options.Create(new JsonOptions()));

        var httpContext = new DefaultHttpContext
        {
            RequestServices = services.BuildServiceProvider(),
        };

        httpContext.Request.Path = path;
        httpContext.Request.QueryString = new QueryString(queryString);
        httpContext.Response.Body = new MemoryStream();

        return httpContext;
    }

    [Fact]
    public async Task InvokeAsync_CacheMiss_ChamaProximoDelegateEGravaRespostaComSucesso()
    {
        var cacheService = new FakeCacheService { ValorParaRetornar = null };
        var httpContext = CriarHttpContext(cacheService);
        var invocationContext = EndpointFilterInvocationContext.Create(httpContext);
        var filtro = new ResponseCacheEndpointFilter("condominio", "lista");

        var proximoFoiChamado = false;

        Task<object?> Next(EndpointFilterInvocationContext _)
        {
            proximoFoiChamado = true;
            return Task.FromResult<object?>(Results.Ok(new { valor = 42 }));
        }

        var resultado = await filtro.InvokeAsync(invocationContext, ctx => new ValueTask<object?>(Next(ctx)));

        Assert.True(proximoFoiChamado);
        Assert.NotNull(resultado);

        Assert.True(cacheService.SetAsyncFoiChamado);
        Assert.Equal("condominio", cacheService.UltimaEntidade);
        Assert.Equal("lista", cacheService.UltimoTipoConsulta);
        Assert.Equal(200, cacheService.UltimoStatusCode);
        Assert.Contains("42", cacheService.UltimaRespostaJson);
        Assert.Equal("/api/condominios", cacheService.UltimaChaveCache);
    }

    [Fact]
    public async Task InvokeAsync_CacheHit_NaoChamaProximoDelegateERetornaValorCacheado()
    {
        const string respostaCacheada = "{\"valor\":99}";

        var cacheService = new FakeCacheService { ValorParaRetornar = (200, respostaCacheada) };
        var httpContext = CriarHttpContext(cacheService);
        var invocationContext = EndpointFilterInvocationContext.Create(httpContext);
        var filtro = new ResponseCacheEndpointFilter("condominio", "lista");

        var proximoFoiChamado = false;

        Task<object?> Next(EndpointFilterInvocationContext _)
        {
            proximoFoiChamado = true;
            return Task.FromResult<object?>(Results.Ok(new { valor = 1 }));
        }

        var resultado = await filtro.InvokeAsync(invocationContext, ctx => new ValueTask<object?>(Next(ctx)));

        Assert.False(proximoFoiChamado);
        Assert.False(cacheService.SetAsyncFoiChamado);

        var result = Assert.IsAssignableFrom<IResult>(resultado);
        await result.ExecuteAsync(httpContext);

        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(httpContext.Response.Body);
        var corpoEscrito = await reader.ReadToEndAsync();

        Assert.Equal(200, httpContext.Response.StatusCode);

        using var jsonDocument = JsonDocument.Parse(corpoEscrito);
        Assert.Equal(99, jsonDocument.RootElement.GetProperty("valor").GetInt32());
    }

    [Fact]
    public async Task InvokeAsync_RespostaDeErro404_NaoGravaCache()
    {
        var cacheService = new FakeCacheService { ValorParaRetornar = null };
        var httpContext = CriarHttpContext(cacheService, path: "/api/condominios/999");
        var invocationContext = EndpointFilterInvocationContext.Create(httpContext);
        var filtro = new ResponseCacheEndpointFilter("condominio", "detalhe");

        Task<object?> Next(EndpointFilterInvocationContext _) =>
            Task.FromResult<object?>(Results.NotFound());

        var resultado = await filtro.InvokeAsync(invocationContext, ctx => new ValueTask<object?>(Next(ctx)));

        Assert.False(cacheService.SetAsyncFoiChamado);
        Assert.IsAssignableFrom<IResult>(resultado);
    }
}
