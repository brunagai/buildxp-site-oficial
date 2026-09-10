using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BuildXP.Tests;

public class ApiSemBancoTests : IClassFixture<ApiSemBancoFactory>
{
    private readonly HttpClient _client;

    public ApiSemBancoTests(ApiSemBancoFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
    }

    [Fact]
    public async Task Health_sobe_sem_migrar_o_banco()
    {
        var resposta = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);

        var corpo = await resposta.Content.ReadFromJsonAsync<HealthDto>();
        Assert.NotNull(corpo);
        Assert.False(string.IsNullOrWhiteSpace(corpo.Status));
    }

    [Fact]
    public async Task Pagina_do_simulador_sobe_sem_migrar_o_banco()
    {
        var resposta = await _client.GetAsync("/simulador.html");
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
    }

    [Fact]
    public async Task Pagina_da_rotina_sobe_sem_migrar_o_banco()
    {
        var resposta = await _client.GetAsync("/rotina.html");
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
    }

    private sealed class HealthDto
    {
        public string Status { get; set; } = string.Empty;
    }
}

public class ApiSemBancoFactory : WebApplicationFactory<Program>
{
    private const string VariavelPularBanco = "BUILDXP_SKIP_DB";
    private readonly string? _valorAnterior;

    public ApiSemBancoFactory()
    {
        _valorAnterior = Environment.GetEnvironmentVariable(VariavelPularBanco);
        Environment.SetEnvironmentVariable(VariavelPularBanco, "1");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.UseSetting("https_port", "");
        builder.UseSetting("Jwt:Chave", new string('k', 32));
    }

    protected override void Dispose(bool disposing)
    {
        Environment.SetEnvironmentVariable(VariavelPularBanco, _valorAnterior);
        base.Dispose(disposing);
    }
}
