using System.Net;
using System.Net.Http.Json;
using CircleERP.Application.Currencies;
using Microsoft.AspNetCore.Mvc;

namespace CircleERP.Api.IntegrationTests.Currencies;

[TestFixture]
public class CurrenciesEndpointsTests
{
    private CircleErpApiFactory _factory = null!;
    private HttpClient _client = null!;

    private static readonly object ValidCurrency = new
    {
        code = "BRL",
        description = "Real brasileiro",
        rate = 1.0m,
        symbol = "R$",
    };

    [SetUp]
    public void SetUp()
    {
        _factory = new CircleErpApiFactory();
        _client = _factory.CreateClient();
        _factory.InitializeDatabase();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    private async Task<int> RegisterAsync(object? currency = null)
    {
        var response = await _client.PostAsJsonAsync("/api/currencies", currency ?? ValidCurrency);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<int>();
    }

    [Test]
    public async Task Listagem_comeca_vazia()
    {
        var currencies = await _client.GetFromJsonAsync<List<CurrencyResponse>>("/api/currencies");

        Assert.That(currencies, Is.Empty);
    }

    [Test]
    public async Task Cadastrar_devolve_201_e_o_recurso_fica_acessivel_pelo_Location()
    {
        var response = await _client.PostAsJsonAsync("/api/currencies", ValidCurrency);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        Assert.That(response.Headers.Location, Is.Not.Null);

        var created = await _client.GetFromJsonAsync<CurrencyResponse>(response.Headers.Location);

        Assert.Multiple(() =>
        {
            Assert.That(created!.Code, Is.EqualTo("BRL"));
            Assert.That(created.Symbol, Is.EqualTo("R$"));
            Assert.That(created.Rate, Is.EqualTo(1.0m));
        });
    }

    [Test]
    public async Task Codigo_e_normalizado_para_maiusculas_ao_cadastrar()
    {
        var id = await RegisterAsync(new { code = "brl", description = "Real", rate = 1.0m });

        var created = await _client.GetFromJsonAsync<CurrencyResponse>($"/api/currencies/{id}");

        Assert.That(created!.Code, Is.EqualTo("BRL"));
    }

    [Test]
    public async Task Moeda_sem_simbolo_e_valida()
    {
        var id = await RegisterAsync(new { code = "JPY", description = "Iene", rate = 0.04m });

        var created = await _client.GetFromJsonAsync<CurrencyResponse>($"/api/currencies/{id}");

        Assert.That(created!.Symbol, Is.Null);
    }

    [Test]
    public async Task Cadastrar_codigo_repetido_devolve_409()
    {
        await RegisterAsync();

        var response = await _client.PostAsJsonAsync("/api/currencies", ValidCurrency);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.That(problem!.Detail, Does.Contain("BRL"));
    }

    [Test]
    public async Task Codigo_repetido_em_caixa_diferente_tambem_conflita()
    {
        // A deteccao depende da normalizacao no value object, e nao de
        // ToUpper() na consulta.
        await RegisterAsync();

        var response = await _client.PostAsJsonAsync(
            "/api/currencies",
            new { code = "brl", description = "Outro", rate = 2.0m });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
    }

    [TestCase("BR", TestName = "codigo curto")]
    [TestCase("BRLL", TestName = "codigo longo")]
    [TestCase("R$", TestName = "codigo com simbolo")]
    [TestCase("", TestName = "codigo vazio")]
    public async Task Codigo_invalido_devolve_400_com_ProblemDetails(string code)
    {
        var response = await _client.PostAsJsonAsync(
            "/api/currencies",
            new { code, description = "Qualquer", rate = 1.0m });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.That(problem!.Detail, Is.Not.Empty);
    }

    [TestCase(0)]
    [TestCase(-1)]
    public async Task Taxa_nao_positiva_devolve_400(decimal rate)
    {
        var response = await _client.PostAsJsonAsync(
            "/api/currencies",
            new { code = "BRL", description = "Real", rate });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Alterar_devolve_204_e_persiste_descricao_taxa_e_simbolo()
    {
        var id = await RegisterAsync();

        var response = await _client.PutAsJsonAsync(
            $"/api/currencies/{id}",
            new { description = "Real", rate = 5.432109m, symbol = "R$" });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var updated = await _client.GetFromJsonAsync<CurrencyResponse>($"/api/currencies/{id}");

        Assert.Multiple(() =>
        {
            Assert.That(updated!.Description, Is.EqualTo("Real"));
            Assert.That(updated.Rate, Is.EqualTo(5.432109m));
            Assert.That(updated.Code, Is.EqualTo("BRL"), "o codigo nao muda em uma alteracao");
        });
    }

    [Test]
    public async Task Alterar_removendo_o_simbolo_deixa_o_campo_nulo()
    {
        var id = await RegisterAsync();

        await _client.PutAsJsonAsync(
            $"/api/currencies/{id}",
            new { description = "Real", rate = 1.0m, symbol = (string?)null });

        var updated = await _client.GetFromJsonAsync<CurrencyResponse>($"/api/currencies/{id}");

        Assert.That(updated!.Symbol, Is.Null);
    }

    [Test]
    public async Task Remover_devolve_204_e_a_moeda_some_da_listagem()
    {
        var id = await RegisterAsync();

        var response = await _client.DeleteAsync($"/api/currencies/{id}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var currencies = await _client.GetFromJsonAsync<List<CurrencyResponse>>("/api/currencies");

        Assert.That(currencies, Is.Empty);
    }

    [TestCase("GET")]
    [TestCase("DELETE")]
    public async Task Operar_sobre_id_inexistente_devolve_404(string method)
    {
        using var request = new HttpRequestMessage(new HttpMethod(method), "/api/currencies/9999");

        var response = await _client.SendAsync(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Alterar_id_inexistente_devolve_404()
    {
        var response = await _client.PutAsJsonAsync(
            "/api/currencies/9999",
            new { description = "Real", rate = 1.0m, symbol = (string?)null });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Rotas_antigas_nao_respondem_mais()
    {
        // Contrato antigo, substituido na Fase 2.
        var response = await _client.GetAsync("/api/currency");

        Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.OK));
    }
}
