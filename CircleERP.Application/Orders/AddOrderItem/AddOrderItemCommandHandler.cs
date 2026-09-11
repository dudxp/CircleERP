using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Application.Abstractions.Persistence;
using CircleERP.Application.Products;
using CircleERP.Domain.Orders;
using CircleERP.Domain.Products;
using FluentResults;

namespace CircleERP.Application.Orders.AddOrderItem;

internal sealed class AddOrderItemCommandHandler(
    IOrderRepository orders,
    IProductRepository products,
    IUnitOfWork unitOfWork) : ICommandHandler<AddOrderItemCommand, int>
{
    public async Task<Result<int>> Handle(
        AddOrderItemCommand command,
        CancellationToken cancellationToken)
    {
        var order = await orders.GetByIdAsync(command.OrderId, cancellationToken);

        if (order is null)
            return Result.Fail<int>(OrderErrors.NotFound(command.OrderId));

        var product = await products.GetByIdAsync(command.ProductId, cancellationToken);

        if (product is null)
            return Result.Fail<int>(ProductErrors.NotFound(command.ProductId));

        if (!product.IsActive)
            return Result.Fail<int>(ProductErrors.Inactive(product.Name.Value));

        // O nome do produto e copiado aqui, no instante da venda. E o caso de uso
        // que faz a copia porque o agregado Order nao alcanca o agregado Product.
        // Se o produto for renomeado depois, este pedido segue dizendo o que
        // realmente vendeu.
        var item = order.AddItem(
            product.Id,
            ItemDescription.Create(product.Name.Value),
            Quantity.Create(command.Quantity),
            command.UnitPrice);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(item.Id);
    }
}
