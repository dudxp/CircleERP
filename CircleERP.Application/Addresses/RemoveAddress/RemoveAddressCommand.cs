using CircleERP.Application.Abstractions.Messaging;

namespace CircleERP.Application.Addresses.RemoveAddress;

public sealed record RemoveAddressCommand(int Id) : ICommand;
