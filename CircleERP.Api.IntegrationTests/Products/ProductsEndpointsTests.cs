using System.Net;
using System.Net.Http.Json;
using CircleERP.Application.Orders;
using CircleERP.Application.Products;
using Microsoft.AspNetCore.Mvc;

namespace CircleERP.Api.IntegrationTests.Products;

[TestFixture]
public class ProductsEndpointsTests
{
    private CircleErpApiFactory _factory = null!;
    private HttpClient _client = null!;

    [SetUp]
    public async Task SetUp()
    {
        _factory = new CircleErpApiFactory();
        _client = _factory.CreateClient();
        _factory.InitializeDatabase();

        // O preco do produto carrega a moeda, entao ela precisa existir.
        var currency = await _client.PostAsJsonAsync(
            "/api/currencies",
            new { code = "BRL", description = "Real brasileiro", rate = 1.0m, symbol = "R$" });

        currency.EnsureSuccessStatusCode();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    private static object Fields(
        string sku = "TEC-001",
        string name = "Teclado mecanico",
        decimal price = 199.90m,
        string currencyCode = "BRL",
        string unit = "Unit") =>
        new { sku, name, price, currencyCode, unit };

    private async Task<int> RegisterAsync(object? fields = null)
    {
        var response = await _client.PostAsJsonAsync("/api/products", fields ?? Fields());
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<int>();
    }

    private Task<ProductResponse?> GetAsync(int id) =>
        _client.GetFromJsonAsync<ProductResponse>($"/api/products/{id}");

    [Test]
    public async Task Produto_cadastrado_nasce_ativo_com_preco_e_moeda()
    {
        var id = await RegisterAsync();

        var product = await GetAsync(id);

        Assert.Multiple(() =>
        {
            Assert.That(product!.IsActive, Is.True);
            Assert.That(product.Price, Is.EqualTo(199.90m));
            Assert.That(product.CurrencyCode, Is.EqualTo("BRL"));
            Assert.That(product.Unit, Is.EqualTo("Unit"));
        });
    }

    [Test]
    public async Task Sku_e_normalizado_para_maiusculas()
    {
        var id = await RegisterAsync(Fields(sku: "tec-001"));

        Assert.That((await GetAsync(id))!.Sku, Is.EqualTo("TEC-001"));
    }

    [Test]
    public async Task Sku_repetido_devolve_409()
    {
        await RegisterAsync();

        var response = await _client.PostAsJsonAsync("/api/products", Fields(name: "Outro"));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
    }

    [Test]
    public async Task Preco_em_moeda_nao_cadastrada_devolve_404()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/products",
            Fields(currencyCode: "JPY"));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.That(problem!.Detail, Does.Contain("JPY"));
    }

    [Test]
    public async Task Unidade_desconhecida_devolve_400()
    {
        var response = await _client.PostAsJsonAsync("/api/products", Fields(unit: "Duzia"));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Sku_com_espaco_devolve_400()
    {
        var response = await _client.PostAsJsonAsync("/api/products", Fields(sku: "TEC 001"));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Preco_negativo_devolve_400()
    {
        var response = await _client.PostAsJsonAsync("/api/products", Fields(price: -1m));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Inativar_e_reativar()
    {
        var id = await RegisterAsync();

        await _client.PostAsync($"/api/products/{id}/deactivate", null);
        Assert.That((await GetAsync(id))!.IsActive, Is.False);

        await _client.PostAsync($"/api/products/{id}/activate", null);
        Assert.That((await GetAsync(id))!.IsActive, Is.True);
    }

    [Test]
    public async Task Nao_existe_exclusao_de_produto()
    {
        var id = await RegisterAsync();

        var response = await _client.DeleteAsync($"/api/products/{id}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.MethodNotAllowed));
    }

    [Test]
    public async Task Produto_inativo_nao_entra_em_pedido()
    {
        var productId = await RegisterAsync();
        var orderId = await OpenOrderAsync();

        await _client.PostAsync($"/api/products/{productId}/deactivate", null);

        var response = await _client.PostAsJsonAsync(
            $"/api/orders/{orderId}/items",
            new { productId, quantity = 1, unitPrice = 10.00m });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
    }

    [Test]
    public async Task Reajustar_o_produto_nao_muda_pedido_ja_feito()
    {
        // A razao de o item congelar nome e preco: o historico nao se reescreve.
        var productId = await RegisterAsync(Fields(price: 100.00m));
        var orderId = await OpenOrderAsync();

        await _client.PostAsJsonAsync(
            $"/api/orders/{orderId}/items",
            new { productId, quantity = 2, unitPrice = 100.00m });

        await _client.PutAsJsonAsync(
            $"/api/products/{productId}",
            Fields(name: "Teclado mecanico RGB", price: 250.00m));

        var order = await _client.GetFromJsonAsync<OrderResponse>($"/api/orders/{orderId}");

        Assert.Multiple(() =>
        {
            Assert.That(order!.Items[0].UnitPrice, Is.EqualTo(100.00m), "o preco praticado nao muda");
            Assert.That(
                order.Items[0].Description,
                Is.EqualTo("Teclado mecanico"),
                "o nome vendido nao muda");
            Assert.That(order.Total, Is.EqualTo(200.00m));
            Assert.That(order.Items[0].ProductId, Is.EqualTo(productId));
        });
    }

    [Test]
    public async Task Produto_inexistente_devolve_404()
    {
        var response = await _client.GetAsync("/api/products/9999");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    private async Task<int> OpenOrderAsync()
    {
        var customer = await _client.PostAsJsonAsync(
            "/api/customers",
            new { name = "Eduardo", type = "Individual", document = "52998224725", addressId = (int?)null });

        customer.EnsureSuccessStatusCode();
        var customerId = await customer.Content.ReadFromJsonAsync<int>();

        var order = await _client.PostAsJsonAsync(
            "/api/orders",
            new { customerId, currencyCode = "BRL" });

        order.EnsureSuccessStatusCode();

        return await order.Content.ReadFromJsonAsync<int>();
    }
}
