using System.Net;
using System.Net.Http.Json;
using CircleERP.Application.Orders;
using Microsoft.AspNetCore.Mvc;

namespace CircleERP.Api.IntegrationTests.Orders;

[TestFixture]
public class OrdersEndpointsTests
{
    private CircleErpApiFactory _factory = null!;
    private HttpClient _client = null!;

    [SetUp]
    public async Task SetUp()
    {
        _factory = new CircleErpApiFactory();
        _client = _factory.CreateClient();
        _factory.InitializeDatabase();

        // Um pedido so pode ser aberto em uma moeda cadastrada.
        var response = await _client.PostAsJsonAsync(
            "/api/currencies",
            new { code = "BRL", description = "Real brasileiro", rate = 1.0m, symbol = "R$" });

        response.EnsureSuccessStatusCode();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    private async Task<int> OpenOrderAsync(string currencyCode = "BRL")
    {
        var response = await _client.PostAsJsonAsync(
            "/api/orders",
            new { customer = "Eduardo", currencyCode });

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<int>();
    }

    private async Task<int> AddItemAsync(
        int orderId,
        string description = "Teclado",
        int quantity = 1,
        decimal unitPrice = 10.00m)
    {
        var response = await _client.PostAsJsonAsync(
            $"/api/orders/{orderId}/items",
            new { description, quantity, unitPrice });

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<int>();
    }

    private Task<OrderResponse?> GetOrderAsync(int orderId) =>
        _client.GetFromJsonAsync<OrderResponse>($"/api/orders/{orderId}");

    [Test]
    public async Task Pedido_aberto_comeca_em_rascunho_sem_itens()
    {
        var id = await OpenOrderAsync();

        var order = await GetOrderAsync(id);

        Assert.Multiple(() =>
        {
            Assert.That(order!.Status, Is.EqualTo("Draft"));
            Assert.That(order.Items, Is.Empty);
            Assert.That(order.Total, Is.Zero);
            Assert.That(order.Currency, Is.EqualTo("BRL"));
            Assert.That(order.PlacedOnUtc, Is.Null);
        });
    }

    [Test]
    public async Task Abrir_pedido_em_moeda_nao_cadastrada_devolve_404()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/orders",
            new { customer = "Eduardo", currencyCode = "JPY" });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.That(problem!.Detail, Does.Contain("JPY"));
    }

    [Test]
    public async Task Itens_sao_persistidos_com_o_pedido_e_o_total_e_calculado()
    {
        var id = await OpenOrderAsync();

        await AddItemAsync(id, "Teclado", quantity: 3, unitPrice: 10.50m);
        await AddItemAsync(id, "Mouse", quantity: 2, unitPrice: 4.25m);

        var order = await GetOrderAsync(id);

        Assert.Multiple(() =>
        {
            Assert.That(order!.Items, Has.Count.EqualTo(2));
            Assert.That(order.Total, Is.EqualTo(40.00m));
            Assert.That(order.Items[0].LineTotal, Is.EqualTo(31.50m));
        });
    }

    [Test]
    public async Task Listagem_traz_o_total_e_a_contagem_de_itens()
    {
        var id = await OpenOrderAsync();
        await AddItemAsync(id, quantity: 2, unitPrice: 5.00m);

        var orders = await _client.GetFromJsonAsync<List<OrderSummaryResponse>>("/api/orders");

        Assert.Multiple(() =>
        {
            Assert.That(orders, Has.Count.EqualTo(1));
            Assert.That(orders![0].Total, Is.EqualTo(10.00m), "o resumo carrega as linhas para somar");
            Assert.That(orders[0].ItemCount, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task Alterar_a_quantidade_recalcula_o_total()
    {
        var id = await OpenOrderAsync();
        var itemId = await AddItemAsync(id, quantity: 1, unitPrice: 7.00m);

        var response = await _client.PutAsJsonAsync(
            $"/api/orders/{id}/items/{itemId}",
            new { quantity = 3 });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var order = await GetOrderAsync(id);

        Assert.That(order!.Total, Is.EqualTo(21.00m));
    }

    [Test]
    public async Task Remover_item_tira_a_linha_e_baixa_o_total()
    {
        var id = await OpenOrderAsync();
        var itemId = await AddItemAsync(id, unitPrice: 10.00m);
        await AddItemAsync(id, "Mouse", unitPrice: 5.00m);

        var response = await _client.DeleteAsync($"/api/orders/{id}/items/{itemId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var order = await GetOrderAsync(id);

        Assert.Multiple(() =>
        {
            Assert.That(order!.Items, Has.Count.EqualTo(1));
            Assert.That(order.Total, Is.EqualTo(5.00m));
        });
    }

    [Test]
    public async Task Confirmar_pedido_sem_itens_devolve_400()
    {
        var id = await OpenOrderAsync();

        var response = await _client.PostAsync($"/api/orders/{id}/place", null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var order = await GetOrderAsync(id);

        Assert.That(order!.Status, Is.EqualTo("Draft"), "a situacao nao muda quando a confirmacao falha");
    }

    [Test]
    public async Task Confirmar_muda_a_situacao_e_registra_a_data()
    {
        var id = await OpenOrderAsync();
        await AddItemAsync(id);

        var response = await _client.PostAsync($"/api/orders/{id}/place", null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var order = await GetOrderAsync(id);

        Assert.Multiple(() =>
        {
            Assert.That(order!.Status, Is.EqualTo("Placed"));
            Assert.That(order.PlacedOnUtc, Is.Not.Null);
        });
    }

    [Test]
    public async Task Pedido_confirmado_nao_aceita_mais_itens()
    {
        var id = await OpenOrderAsync();
        await AddItemAsync(id);
        await _client.PostAsync($"/api/orders/{id}/place", null);

        var response = await _client.PostAsJsonAsync(
            $"/api/orders/{id}/items",
            new { description = "Monitor", quantity = 1, unitPrice = 100.00m });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Cancelar_muda_a_situacao_e_cancelar_de_novo_devolve_400()
    {
        var id = await OpenOrderAsync();

        var first = await _client.PostAsync($"/api/orders/{id}/cancel", null);
        var second = await _client.PostAsync($"/api/orders/{id}/cancel", null);

        Assert.Multiple(() =>
        {
            Assert.That(first.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
            Assert.That(second.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        });
    }

    [TestCase(0)]
    [TestCase(-1)]
    public async Task Quantidade_nao_positiva_devolve_400(int quantity)
    {
        var id = await OpenOrderAsync();

        var response = await _client.PostAsJsonAsync(
            $"/api/orders/{id}/items",
            new { description = "Teclado", quantity, unitPrice = 10.00m });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Preco_com_mais_de_duas_casas_devolve_400()
    {
        var id = await OpenOrderAsync();

        var response = await _client.PostAsJsonAsync(
            $"/api/orders/{id}/items",
            new { description = "Teclado", quantity = 1, unitPrice = 10.999m });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Item_de_outro_pedido_devolve_400()
    {
        var first = await OpenOrderAsync();
        var second = await OpenOrderAsync();
        var itemId = await AddItemAsync(first);

        var response = await _client.DeleteAsync($"/api/orders/{second}/items/{itemId}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Operar_sobre_pedido_inexistente_devolve_404()
    {
        var get = await _client.GetAsync("/api/orders/9999");
        var place = await _client.PostAsync("/api/orders/9999/place", null);

        Assert.Multiple(() =>
        {
            Assert.That(get.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(place.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        });
    }

    [Test]
    public async Task Preco_do_item_e_gravado_na_moeda_do_pedido()
    {
        var id = await OpenOrderAsync();
        await AddItemAsync(id, unitPrice: 12.34m);

        var order = await GetOrderAsync(id);

        Assert.Multiple(() =>
        {
            Assert.That(order!.Items[0].UnitPrice, Is.EqualTo(12.34m));
            Assert.That(order.Currency, Is.EqualTo("BRL"));
        });
    }
}
