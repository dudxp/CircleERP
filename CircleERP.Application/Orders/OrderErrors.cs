using CircleERP.Application.Abstractions.Errors;
using FluentResults;

namespace CircleERP.Application.Orders;

internal static class OrderErrors
{
    internal static Error NotFound(int id) =>
        new NotFoundError($"Pedido {id} nao encontrado.");

    internal static Error CurrencyNotRegistered(string code) =>
        new NotFoundError($"Moeda {code} nao esta cadastrada.");
}
