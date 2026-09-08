using CircleERP.Application.Abstractions.Messaging;

namespace CircleERP.Application.Orders.GetOrders;

public sealed record GetOrdersQuery : IQuery<IReadOnlyList<OrderSummaryResponse>>;
