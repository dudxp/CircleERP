using CircleERP.Application.Abstractions.Messaging;

namespace CircleERP.Application.Customers.ChangeCustomer;

public sealed record ChangeCustomerCommand(int Id, CustomerFields Fields) : ICommand;
