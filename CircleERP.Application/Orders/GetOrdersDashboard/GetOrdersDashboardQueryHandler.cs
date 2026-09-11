using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Application.Abstractions.Reporting;
using FluentResults;

namespace CircleERP.Application.Orders.GetOrdersDashboard;

internal sealed class GetOrdersDashboardQueryHandler(IOrderDashboardReader reader)
    : IQueryHandler<GetOrdersDashboardQuery, OrderDashboard>
{
    public async Task<Result<OrderDashboard>> Handle(
        GetOrdersDashboardQuery query,
        CancellationToken cancellationToken) =>
        Result.Ok(await reader.ReadAsync(cancellationToken));
}
