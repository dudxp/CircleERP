using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Application.Abstractions.Persistence;
using CircleERP.Domain.Addresses;
using FluentResults;

namespace CircleERP.Application.Addresses.ChangeAddress;

internal sealed class ChangeAddressCommandHandler(
    IAddressRepository addresses,
    IUnitOfWork unitOfWork) : ICommandHandler<ChangeAddressCommand>
{
    public async Task<Result> Handle(
        ChangeAddressCommand command,
        CancellationToken cancellationToken)
    {
        var address = await addresses.GetByIdAsync(command.Id, cancellationToken);

        if (address is null)
            return Result.Fail(AddressErrors.NotFound(command.Id));

        var (zipCode, street, number, complement, district, city, state) =
            command.Fields.ToValueObjects();

        address.Change(zipCode, street, number, complement, district, city, state);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
