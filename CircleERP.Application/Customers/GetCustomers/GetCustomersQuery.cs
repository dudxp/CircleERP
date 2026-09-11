using CircleERP.Application.Abstractions.Messaging;

namespace CircleERP.Application.Customers.GetCustomers;

public sealed record GetCustomersQuery : IQuery<IReadOnlyList<CustomerResponse>>;
