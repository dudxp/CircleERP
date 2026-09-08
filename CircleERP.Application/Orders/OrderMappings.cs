using CircleERP.Domain.Orders;

namespace CircleERP.Application.Orders;

internal static class OrderMappings
{
    internal static OrderSummaryResponse ToSummary(this Order order) =>
        new(
            order.Id,
            order.Customer.Value,
            order.Currency.Value,
            order.Status.ToString(),
            order.Total.Amount,
            order.Items.Count,
            order.CreatedOnUtc);

    internal static OrderResponse ToResponse(this Order order) =>
        new(
            order.Id,
            order.Customer.Value,
            order.Currency.Value,
            order.Status.ToString(),
            order.Total.Amount,
            order.CreatedOnUtc,
            order.PlacedOnUtc,
            [.. order.Items.Select(item => item.ToResponse())]);

    private static OrderItemResponse ToResponse(this OrderItem item) =>
        new(
            item.Id,
            item.Description.Value,
            item.Quantity.Value,
            item.UnitPrice.Amount,
            item.LineTotal.Amount);
}
