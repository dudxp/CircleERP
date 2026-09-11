using CircleERP.Domain.Customers;
using CircleERP.Domain.Orders;

namespace CircleERP.Application.Orders;

internal static class OrderMappings
{
    internal static OrderSummaryResponse ToSummary(this Order order, string customerName) =>
        new(
            order.Id,
            order.CustomerId,
            customerName,
            order.Currency.Value,
            order.Status.ToString(),
            order.Total.Amount,
            order.Items.Count,
            order.CreatedOnUtc);

    internal static OrderResponse ToResponse(this Order order, string customerName) =>
        new(
            order.Id,
            order.CustomerId,
            customerName,
            order.Currency.Value,
            order.Status.ToString(),
            order.Total.Amount,
            order.CreatedOnUtc,
            order.PlacedOnUtc,
            [.. order.Items.Select(item => item.ToResponse())]);

    private static OrderItemResponse ToResponse(this OrderItem item) =>
        new(
            item.Id,
            item.ProductId,
            item.Description.Value,
            item.Quantity.Value,
            item.UnitPrice.Amount,
            item.LineTotal.Amount);
}

internal static class OrderCustomerNames
{
    /// <summary>
    /// Nome do cliente para exibicao. Um pedido cujo cliente sumiu do cadastro
    /// ainda precisa ser legivel, entao a ausencia vira um rotulo em vez de
    /// quebrar a listagem.
    /// </summary>
    internal static string NameOf(this IReadOnlyDictionary<int, Customer> customers, int customerId) =>
        customers.TryGetValue(customerId, out var customer)
            ? customer.Name.Value
            : $"(cliente {customerId})";
}
