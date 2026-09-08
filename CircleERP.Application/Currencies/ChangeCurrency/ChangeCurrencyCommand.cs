using CircleERP.Application.Abstractions.Messaging;

namespace CircleERP.Application.Currencies.ChangeCurrency;

/// <summary>
/// O codigo nao entra: ele identifica a moeda. Trocar o codigo de uma moeda
/// existente seria cadastrar outra moeda.
/// </summary>
public sealed record ChangeCurrencyCommand(
    int Id,
    string Description,
    decimal Rate,
    string? Symbol = null) : ICommand;
