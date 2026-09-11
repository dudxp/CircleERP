using System.Net;
using System.Net.Http.Json;
using CircleERP.Application.Addresses;
using CircleERP.Application.Customers;
using Microsoft.AspNetCore.Mvc;

namespace CircleERP.Api.IntegrationTests.Customers;

[TestFixture]
public class CustomersEndpointsTests
{
    private const string ValidCpf = "52998224725";
    private const string AnotherValidCpf = "39053344705";
    private const string ValidCnpj = "11222333000181";

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
        string document = ValidCpf,
        string type = "Individual",
        string name = "Eduardo",
        int? addressId = null) =>
        new { name, type, document, addressId };

    private async Task<int> RegisterAsync(object? fields = null)
    {
        var response = await _client.PostAsJsonAsync("/api/customers", fields ?? Fields());
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<int>();
    }

    private async Task<int> RegisterAddressAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/addresses", new
        {
            zipCode = "01310-100",
            street = "Avenida Paulista",
            number = "1578",
            complement = (string?)null,
            district = "Bela Vista",
            city = "Sao Paulo",
            state = "SP",
        });

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<int>();
    }

    private Task<CustomerResponse?> GetAsync(int id) =>
        _client.GetFromJsonAsync<CustomerResponse>($"/api/customers/{id}");

    [Test]
    public async Task Cliente_cadastrado_nasce_ativo_e_sem_endereco()
    {
        var id = await RegisterAsync();

        var customer = await GetAsync(id);

        Assert.Multiple(() =>
        {
            Assert.That(customer!.IsActive, Is.True);
            Assert.That(customer.AddressId, Is.Null);
            Assert.That(customer.Address, Is.Null);
            Assert.That(customer.Type, Is.EqualTo("Individual"));
        });
    }

    [Test]
    public async Task Documento_e_gravado_sem_pontuacao_e_devolvido_formatado()
    {
        var id = await RegisterAsync(Fields(document: "529.982.247-25"));

        var customer = await GetAsync(id);

        Assert.Multiple(() =>
        {
            Assert.That(customer!.Document, Is.EqualTo(ValidCpf));
            Assert.That(customer.FormattedDocument, Is.EqualTo("529.982.247-25"));
        });
    }

    [Test]
    public async Task Tipo_acompanha_o_documento()
    {
        var id = await RegisterAsync(Fields(document: ValidCnpj, type: "Company", name: "Empresa LTDA"));

        var customer = await GetAsync(id);

        Assert.That(customer!.Type, Is.EqualTo("Company"));
    }

    [Test]
    public async Task Documento_repetido_devolve_409()
    {
        await RegisterAsync();

        var response = await _client.PostAsJsonAsync("/api/customers", Fields());

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
    }

    [Test]
    public async Task Mesmo_documento_com_e_sem_pontuacao_e_duplicata()
    {
        await RegisterAsync(Fields(document: ValidCpf));

        var response = await _client.PostAsJsonAsync(
            "/api/customers",
            Fields(document: "529.982.247-25"));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
    }

    [Test]
    public async Task Cpf_invalido_devolve_400()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/customers",
            Fields(document: "52998224726"));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.That(problem!.Detail, Does.Contain("CPF"));
    }

    [Test]
    public async Task Cpf_declarado_como_pessoa_juridica_devolve_400()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/customers",
            Fields(document: ValidCpf, type: "Company"));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Tipo_desconhecido_devolve_400()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/customers",
            Fields(type: "Outro"));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Vincular_endereco_traz_ele_resolvido_na_leitura()
    {
        var addressId = await RegisterAddressAsync();

        var id = await RegisterAsync(Fields(addressId: addressId));

        var customer = await GetAsync(id);

        Assert.Multiple(() =>
        {
            Assert.That(customer!.AddressId, Is.EqualTo(addressId));
            Assert.That(customer.Address, Is.Not.Null);
            Assert.That(
                customer.Address!.SingleLine,
                Is.EqualTo("Avenida Paulista, 1578 - Bela Vista, Sao Paulo/SP"));
        });
    }

    [Test]
    public async Task Vincular_endereco_inexistente_devolve_404()
    {
        var response = await _client.PostAsJsonAsync("/api/customers", Fields(addressId: 9999));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Alterar_mantendo_o_proprio_documento_nao_e_duplicata()
    {
        var id = await RegisterAsync();

        var response = await _client.PutAsJsonAsync(
            $"/api/customers/{id}",
            Fields(name: "Eduardo Silva"));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var customer = await GetAsync(id);

        Assert.That(customer!.Name, Is.EqualTo("Eduardo Silva"));
    }

    [Test]
    public async Task Alterar_para_documento_de_outro_cliente_devolve_409()
    {
        await RegisterAsync(Fields(document: ValidCpf));
        var second = await RegisterAsync(Fields(document: AnotherValidCpf, name: "Outro"));

        var response = await _client.PutAsJsonAsync(
            $"/api/customers/{second}",
            Fields(document: ValidCpf, name: "Outro"));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
    }

    [Test]
    public async Task Desvincular_endereco_ao_alterar()
    {
        var addressId = await RegisterAddressAsync();
        var id = await RegisterAsync(Fields(addressId: addressId));

        await _client.PutAsJsonAsync($"/api/customers/{id}", Fields(addressId: null));

        var customer = await GetAsync(id);

        Assert.That(customer!.AddressId, Is.Null);
    }

    [Test]
    public async Task Inativar_e_reativar()
    {
        var id = await RegisterAsync();

        var deactivate = await _client.PostAsync($"/api/customers/{id}/deactivate", null);
        Assert.That(deactivate.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        Assert.That((await GetAsync(id))!.IsActive, Is.False);

        var activate = await _client.PostAsync($"/api/customers/{id}/activate", null);
        Assert.That(activate.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        Assert.That((await GetAsync(id))!.IsActive, Is.True);
    }

    [Test]
    public async Task Inativar_duas_vezes_devolve_400()
    {
        var id = await RegisterAsync();

        await _client.PostAsync($"/api/customers/{id}/deactivate", null);
        var second = await _client.PostAsync($"/api/customers/{id}/deactivate", null);

        Assert.That(second.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Nao_existe_exclusao_de_cliente()
    {
        // Cliente com historico nao se apaga, se inativa.
        var id = await RegisterAsync();

        var response = await _client.DeleteAsync($"/api/customers/{id}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.MethodNotAllowed));
    }

    [Test]
    public async Task Cliente_inexistente_devolve_404()
    {
        var response = await _client.GetAsync("/api/customers/9999");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Listagem_resolve_o_endereco_de_cada_cliente()
    {
        var addressId = await RegisterAddressAsync();
        await RegisterAsync(Fields(document: ValidCpf, addressId: addressId));
        await RegisterAsync(Fields(document: AnotherValidCpf, name: "Sem endereco"));

        var customers = await _client.GetFromJsonAsync<List<CustomerResponse>>("/api/customers");

        Assert.Multiple(() =>
        {
            Assert.That(customers, Has.Count.EqualTo(2));
            Assert.That(customers![0].Address, Is.Not.Null);
            Assert.That(customers[1].Address, Is.Null);
        });
    }
}
