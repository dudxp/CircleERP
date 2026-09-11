using CircleERP.Application.Abstractions.Messaging;

namespace CircleERP.Application.Addresses.ChangeAddress;

public sealed record ChangeAddressCommand(int Id, AddressFields Fields) : ICommand;
