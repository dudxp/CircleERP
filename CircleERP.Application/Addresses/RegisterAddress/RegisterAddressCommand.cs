using CircleERP.Application.Abstractions.Messaging;

namespace CircleERP.Application.Addresses.RegisterAddress;

public sealed record RegisterAddressCommand(AddressFields Fields) : ICommand<int>;
