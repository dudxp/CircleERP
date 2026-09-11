using System.Net;
using System.Net.Http.Json;
using CircleERP.Application.Abstractions.ZipCodes;
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

    [Test]
    public async Task Cadastrar_endereco_identico_devolve_409_com_o_id_do_existente()
    {
        var existingId = await RegisterAsync();

        var response = await _client.PostAsJsonAsync("/api/addresses", Fields());

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));

        // O id vem nas extensoes do ProblemDetails para a tela poder oferecer o
        // vinculo, em vez de so dizer que houve conflito.
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.That(
            problem!.Extensions["existingAddressId"]!.ToString(),
            Is.EqualTo(existingId.ToString()));
    }

    [Test]
    public async Task Complemento_diferente_nao_e_duplicata()
    {
        await RegisterAsync();

        var response = await _client.PostAsJsonAsync(
            "/api/addresses",
            Fields(complement: "Apto 42"));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task Alterar_um_endereco_para_ficar_igual_a_outro_devolve_409()
    {
        await RegisterAsync();
        var second = await RegisterAsync(Fields(complement: "Apto 42"));

        var response = await _client.PutAsJsonAsync($"/api/addresses/{second}", Fields());

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
    }

    [Test]
    public async Task Alterar_mantendo_os_proprios_dados_nao_e_duplicata()
    {
        var id = await RegisterAsync();

        var response = await _client.PutAsJsonAsync($"/api/addresses/{id}", Fields());

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task Consultar_cep_devolve_o_endereco()
    {
        _factory.ZipCodeLookup.Result = new ZipCodeLookupResult(
            "01310100", "Avenida Paulista", "Bela Vista", "Sao Paulo", "SP");

        var result = await _client.GetFromJsonAsync<ZipCodeLookupResult>(
            "/api/addresses/lookup/01310-100");

        Assert.Multiple(() =>
        {
            Assert.That(result!.Street, Is.EqualTo("Avenida Paulista"));
            Assert.That(result.City, Is.EqualTo("Sao Paulo"));
            Assert.That(result.State, Is.EqualTo("SP"));
        });
    }

    [Test]
    public async Task Consultar_cep_inexistente_devolve_404()
    {
        _factory.ZipCodeLookup.Result = null;

        var response = await _client.GetAsync("/api/addresses/lookup/99999999");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Consultar_cep_com_formato_invalido_nem_chama_o_servico()
    {
        // O value object recusa antes: nao vale gastar uma requisicao externa
        // para descobrir que "123" nao e um CEP.
        _factory.ZipCodeLookup.IsUnavailable = true;

        var response = await _client.GetAsync("/api/addresses/lookup/123");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Servico_de_cep_fora_do_ar_devolve_503()
    {
        _factory.ZipCodeLookup.IsUnavailable = true;

        var response = await _client.GetAsync("/api/addresses/lookup/01310-100");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.ServiceUnavailable));

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.That(problem!.Detail, Does.Contain("manualmente"));
    }
}
