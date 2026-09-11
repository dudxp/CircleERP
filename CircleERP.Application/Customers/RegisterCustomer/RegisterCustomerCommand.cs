using CircleERP.Application.Abstractions.Messaging;

namespace CircleERP.Application.Customers.RegisterCustomer;

public sealed record RegisterCustomerCommand(CustomerFields Fields) : ICommand<int>;
