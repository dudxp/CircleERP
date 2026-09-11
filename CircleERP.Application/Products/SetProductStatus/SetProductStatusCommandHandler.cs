using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Application.Abstractions.Persistence;
using CircleERP.Domain.Products;
using FluentResults;

namespace CircleERP.Application.Products.SetProductStatus;

internal sealed class SetProductStatusCommandHandler(
    IProductRepository products,
    IUnitOfWork unitOfWork) : ICommandHandler<SetProductStatusCommand>
{
    public async Task<Result> Handle(
        SetProductStatusCommand command,
        CancellationToken cancellationToken)
    {
        var product = await products.GetByIdAsync(command.Id, cancellationToken);

        if (product is null)
            return Result.Fail(ProductErrors.NotFound(command.Id));

        if (command.Active)
            product.Activate();
        else
            product.Deactivate();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
