using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Application.Abstractions.Persistence;
using CircleERP.Domain.Addresses;
using FluentResults;

namespace CircleERP.Application.Addresses.RegisterAddress;

internal sealed class RegisterAddressCommandHandler(
    IAddressRepository addresses,
    IUnitOfWork unitOfWork) : ICommandHandler<RegisterAddressCommand, int>
{
    public async Task<Result<int>> Handle(
        RegisterAddressCommand command,
        CancellationToken cancellationToken)
    {
        var (zipCode, street, number, complement, district, city, state) =
            command.Fields.ToValueObjects();

        var address = Address.Register(zipCode, street, number, complement, district, city, state);

        addresses.Add(address);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(address.Id);
    }
}
