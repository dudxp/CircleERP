using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Application.Abstractions.Persistence;
using CircleERP.Domain.Addresses;
using CircleERP.Domain.Customers;
using FluentResults;

namespace CircleERP.Application.Addresses.RemoveAddress;

internal sealed class RemoveAddressCommandHandler(
    IAddressRepository addresses,
    ICustomerRepository customers,
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveAddressCommand>
{
    public async Task<Result> Handle(
        RemoveAddressCommand command,
        CancellationToken cancellationToken)
    {
        var address = await addresses.GetByIdAsync(command.Id, cancellationToken);

        if (address is null)
            return Result.Fail(AddressErrors.NotFound(command.Id));

        // O cliente referencia o endereco por identidade, sem chave estrangeira,
        // entao nada no banco impede a exclusao. A integridade e garantida aqui.
        if (await customers.AnyLinkedToAddressAsync(command.Id, cancellationToken))
            return Result.Fail(AddressErrors.LinkedToCustomer(command.Id));

        addresses.Remove(address);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
