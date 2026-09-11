using System.Net;
using System.Net.Http.Json;
using CircleERP.Application.Addresses;
using Microsoft.AspNetCore.Mvc;

namespace CircleERP.Api.IntegrationTests.Addresses;

[TestFixture]
public class AddressesEndpointsTests
{
    private CircleErpApiFactory _factory = null!;
    private HttpClient _client = null!;

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

    private static object Fields(
        string zipCode = "01310-100",
        string state = "SP",
        string? complement = null) =>
        new
        {
            zipCode,
            street = "Avenida Paulista",
            number = "1578",
            complement,
            district = "Bela Vista",
            city = "Sao Paulo",
            state,
        };

    private async Task<int> RegisterAsync(object? fields = null)
    {
        var response = await _client.PostAsJsonAsync("/api/addresses", fields ?? Fields());
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<int>();
    }

    private Task<AddressResponse?> GetAsync(int id) =>
        _client.GetFromJsonAsync<AddressResponse>($"/api/addresses/{id}");

    [Test]
    public async Task Cep_e_gravado_sem_pontuacao_e_devolvido_formatado()
    {
        var id = await RegisterAsync();

        var address = await GetAsync(id);

        Assert.Multiple(() =>
        {
            Assert.That(address!.ZipCode, Is.EqualTo("01310100"));
            Assert.That(address.FormattedZipCode, Is.EqualTo("01310-100"));
        });
    }

    [Test]
    public async Task Uf_e_normalizada_para_maiusculas()
    {
        var id = await RegisterAsync(Fields(state: "sp"));

        Assert.That((await GetAsync(id))!.State, Is.EqualTo("SP"));
    }

    [Test]
    public async Task Endereco_em_uma_linha_vem_pronto_para_exibicao()
    {
        var id = await RegisterAsync(Fields(complement: "Apto 42"));

        Assert.That(
            (await GetAsync(id))!.SingleLine,
            Is.EqualTo("Avenida Paulista, 1578 - Apto 42 - Bela Vista, Sao Paulo/SP"));
    }

    [Test]
    public async Task Complemento_e_opcional()
    {
        var id = await RegisterAsync();

        Assert.That((await GetAsync(id))!.Complement, Is.Null);
    }

    [TestCase("0131010")]
    [TestCase("013101000")]
    public async Task Cep_fora_de_oito_digitos_devolve_400(string zipCode)
    {
        var response = await _client.PostAsJsonAsync("/api/addresses", Fields(zipCode: zipCode));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Uf_inexistente_devolve_400()
    {
        var response = await _client.PostAsJsonAsync("/api/addresses", Fields(state: "XX"));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.That(problem!.Detail, Does.Contain("UF"));
    }

    [Test]
    public async Task Alterar_devolve_204_e_persiste()
    {
        var id = await RegisterAsync();

        var response = await _client.PutAsJsonAsync(
            $"/api/addresses/{id}",
            Fields(zipCode: "20040-020", state: "RJ"));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var address = await GetAsync(id);

        Assert.Multiple(() =>
        {
            Assert.That(address!.ZipCode, Is.EqualTo("20040020"));
            Assert.That(address.State, Is.EqualTo("RJ"));
        });
    }

    [Test]
    public async Task Remover_endereco_sem_vinculo_devolve_204()
    {
        var id = await RegisterAsync();

        var response = await _client.DeleteAsync($"/api/addresses/{id}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task Remover_endereco_vinculado_a_cliente_devolve_409()
    {
        // Nao ha chave estrangeira entre customer e address: a integridade e
        // garantida pelo caso de uso, e este teste e quem prova isso.
        var addressId = await RegisterAsync();

        await _client.PostAsJsonAsync("/api/customers", new
        {
            name = "Eduardo",
            type = "Individual",
            document = "52998224725",
            addressId,
        });

        var response = await _client.DeleteAsync($"/api/addresses/{addressId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.That(problem!.Detail, Does.Contain("vinculado"));
    }

    [Test]
    public async Task Endereco_inexistente_devolve_404()
    {
        var get = await _client.GetAsync("/api/addresses/9999");
        var remove = await _client.DeleteAsync("/api/addresses/9999");

        Assert.Multiple(() =>
        {
            Assert.That(get.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(remove.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        });
    }

    [Test]
    public async Task Listagem_comeca_vazia()
    {
        var addresses = await _client.GetFromJsonAsync<List<AddressResponse>>("/api/addresses");

        Assert.That(addresses, Is.Empty);
    }
}
