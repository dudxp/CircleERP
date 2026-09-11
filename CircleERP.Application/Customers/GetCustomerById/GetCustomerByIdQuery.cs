using CircleERP.Application.Abstractions.Messaging;

namespace CircleERP.Application.Customers.GetCustomerById;

public sealed record GetCustomerByIdQuery(int Id) : IQuery<CustomerResponse>;
