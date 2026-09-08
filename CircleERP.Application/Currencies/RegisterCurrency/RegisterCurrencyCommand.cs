using CircleERP.Application.Abstractions.Messaging;

namespace CircleERP.Application.Currencies.RegisterCurrency;

public sealed record RegisterCurrencyCommand(string Code, string Description, decimal Rate)
    : ICommand<int>;
