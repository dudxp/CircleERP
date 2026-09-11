using System.Net.Http.Json;
using CircleERP.Application.Abstractions.Reporting;

namespace CircleERP.Api.IntegrationTests.Orders;

[TestFixture]
public class OrdersDashboardTests
{
    private CircleErpApiFactory _factory = null!;
    private HttpClient _client = null!;
    private int _productId;

    [SetUp]
    public async Task SetUp()
    {
        _factory = new CircleErpApiFactory();
        _client = _factory.CreateClient();
        _factory.InitializeDatabase();

        await PostAsync("/api/currencies",
            new { code = "BRL", description = "Real", rate = 1.0m, symbol = "R$" });
        await PostAsync("/api/currencies",
            new { code = "USD", description = "Dolar", rate = 5.0m, symbol = "US$" });

        _productId = await PostAsync("/api/products",
            new { sku = "TEC-001", name = "Teclado", price = 10.00m, currencyCode = "BRL", unit = "Unit" });
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    private async Task<int> PostAsync(string route, object body)
    {
        var response = await _client.PostAsJsonAsync(route, body);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<int>();
    }

    private Task<int> RegisterCustomerAsync(string document, string name) =>
        PostAsync("/api/customers",
            new { name, type = "Individual", document, addressId = (int?)null });

    private Task<int> OpenOrderAsync(int customerId, string currencyCode = "BRL") =>
        PostAsync("/api/orders", new { customerId, currencyCode });

    private Task<OrderDashboard?> ReadDashboardAsync() =>
        _client.GetFromJsonAsync<OrderDashboard>("/api/orders/dashboard");

    [Test]
    public async Task Painel_vazio_nao_quebra()
    {
        var dashboard = await ReadDashboardAsync();

        Assert.Multiple(() =>
        {
            Assert.That(dashboard!.TotalOrders, Is.Zero);
            Assert.That(dashboard.ByCustomer, Is.Empty);
            Assert.That(dashboard.ByDate, Is.Empty);
            Assert.That(dashboard.TotalsByCurrency, Is.Empty);
        });
    }

    [Test]
    public async Task Conta_pedidos_por_cliente_com_o_nome_resolvido()
    {
        var first = await RegisterCustomerAsync("52998224725", "Eduardo");
        var second = await RegisterCustomerAsync("39053344705", "Maria");

        await OpenOrderAsync(first);
        await OpenOrderAsync(first);
        await OpenOrderAsync(second);

        var dashboard = await ReadDashboardAsync();

        Assert.Multiple(() =>
        {
            Assert.That(dashboard!.TotalOrders, Is.EqualTo(3));
            Assert.That(dashboard.ByCustomer, Has.Count.EqualTo(2));
            // Ordenado do maior para o menor, para o grafico sair legivel.
            Assert.That(dashboard.ByCustomer[0].Customer, Is.EqualTo("Eduardo"));
            Assert.That(dashboard.ByCustomer[0].OrderCount, Is.EqualTo(2));
            Assert.That(dashboard.ByCustomer[1].OrderCount, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task Conta_pedidos_por_situacao()
    {
        var customerId = await RegisterCustomerAsync("52998224725", "Eduardo");

        var toCancel = await OpenOrderAsync(customerId);
        await OpenOrderAsync(customerId);

        await _client.PostAsync($"/api/orders/{toCancel}/cancel", null);

        var dashboard = await ReadDashboardAsync();

        var byStatus = dashboard!.ByStatus.ToDictionary(row => row.Status, row => row.OrderCount);

        Assert.Multiple(() =>
        {
            Assert.That(byStatus["Draft"], Is.EqualTo(1));
            Assert.That(byStatus["Cancelled"], Is.EqualTo(1));
        });
    }

    [Test]
    public async Task Agrupa_pedidos_por_data_de_abertura()
    {
        var customerId = await RegisterCustomerAsync("52998224725", "Eduardo");

        await OpenOrderAsync(customerId);
        await OpenOrderAsync(customerId);

        var dashboard = await ReadDashboardAsync();

        Assert.Multiple(() =>
        {
            Assert.That(dashboard!.ByDate, Has.Count.EqualTo(1), "abertos no mesmo dia");
            Assert.That(dashboard.ByDate[0].OrderCount, Is.EqualTo(2));
            Assert.That(dashboard.ByDate[0].Date, Is.EqualTo(DateOnly.FromDateTime(DateTime.UtcNow)));
        });
    }

    [Test]
    public async Task Totais_ficam_separados_por_moeda()
    {
        // A regra do Money vale no painel tambem: nao existe um total que some
        // reais com dolares.
        var customerId = await RegisterCustomerAsync("52998224725", "Eduardo");

        var inReais = await OpenOrderAsync(customerId, "BRL");
        await _client.PostAsJsonAsync(
            $"/api/orders/{inReais}/items",
            new { productId = _productId, quantity = 3, unitPrice = 10.00m });

        var inDollars = await OpenOrderAsync(customerId, "USD");
        await _client.PostAsJsonAsync(
            $"/api/orders/{inDollars}/items",
            new { productId = _productId, quantity = 1, unitPrice = 7.00m });

        var dashboard = await ReadDashboardAsync();

        var totals = dashboard!.TotalsByCurrency.ToDictionary(row => row.Currency);

        Assert.Multiple(() =>
        {
            Assert.That(totals["BRL"].Total, Is.EqualTo(30.00m));
            Assert.That(totals["USD"].Total, Is.EqualTo(7.00m));
            Assert.That(totals, Has.Count.EqualTo(2));
        });
    }

    [Test]
    public async Task Pedido_de_cliente_que_sumiu_do_cadastro_nao_quebra_o_painel()
    {
        var customerId = await RegisterCustomerAsync("52998224725", "Eduardo");
        await OpenOrderAsync(customerId);

        var dashboard = await ReadDashboardAsync();

        Assert.That(dashboard!.ByCustomer[0].Customer, Is.Not.Empty);
    }
}
