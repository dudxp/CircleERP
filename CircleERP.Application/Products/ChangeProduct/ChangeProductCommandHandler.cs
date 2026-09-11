using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Application.Abstractions.Persistence;
using CircleERP.Application.Currencies;
using CircleERP.Domain.Currencies;
using CircleERP.Domain.Products;
using FluentResults;

namespace CircleERP.Application.Products.ChangeProduct;

internal sealed class ChangeProductCommandHandler(
    IProductRepository products,
    ICurrencyRepository currencies,
    IUnitOfWork unitOfWork) : ICommandHandler<ChangeProductCommand>
{
    public async Task<Result> Handle(
        ChangeProductCommand command,
        CancellationToken cancellationToken)
    {
        var product = await products.GetByIdAsync(command.Id, cancellationToken);

        if (product is null)
            return Result.Fail(ProductErrors.NotFound(command.Id));

        var (sku, name, currencyCode, unit) = command.Fields.ToValueObjects();

        if (await products.ExistsWithSkuAsync(sku, command.Id, cancellationToken))
            return Result.Fail(ProductErrors.SkuAlreadyRegistered(sku.Value));

        if (await currencies.GetByCodeAsync(currencyCode, cancellationToken) is null)
            return Result.Fail(CurrencyErrors.NotFoundByCode(currencyCode.Value));

        // Reajustar aqui nao mexe em pedido nenhum: a linha do pedido guarda o
        // preco praticado na venda.
        product.Change(sku, name, command.Fields.ToPrice(currencyCode), unit);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
