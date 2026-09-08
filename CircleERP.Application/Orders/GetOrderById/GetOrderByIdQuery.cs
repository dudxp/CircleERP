using CircleERP.Application.Abstractions.Messaging;

namespace CircleERP.Application.Orders.GetOrderById;

public sealed record GetOrderByIdQuery(int Id) : IQuery<OrderResponse>;
