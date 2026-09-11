using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Application.Abstractions.Persistence;
using CircleERP.Application.Currencies;
using CircleERP.Domain.Currencies;
using CircleERP.Domain.Products;
using FluentResults;

namespace CircleERP.Application.Products.RegisterProduct;

internal sealed class RegisterProductCommandHandler(
    IProductRepository products,
    ICurrencyRepository currencies,
    IUnitOfWork unitOfWork) : ICommandHandler<RegisterProductCommand, int>
{
    public async Task<Result<int>> Handle(
        RegisterProductCommand command,
        CancellationToken cancellationToken)
    {
        var (sku, name, currencyCode, unit) = command.Fields.ToValueObjects();

        if (await products.ExistsWithSkuAsync(sku, cancellationToken))
            return Result.Fail<int>(ProductErrors.SkuAlreadyRegistered(sku.Value));

        // A moeda do preco precisa estar cadastrada: regra entre agregados.
        if (await currencies.GetByCodeAsync(currencyCode, cancellationToken) is null)
            return Result.Fail<int>(CurrencyErrors.NotFoundByCode(currencyCode.Value));

        var product = Product.Register(sku, name, command.Fields.ToPrice(currencyCode), unit);

        products.Add(product);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(product.Id);
    }
}
