using CircleERP.Application.Abstractions.Messaging;

namespace CircleERP.Application.Currencies.RemoveCurrency;

public sealed record RemoveCurrencyCommand(int Id) : ICommand;
