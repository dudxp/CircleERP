using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Domain.Addresses;
using FluentResults;

namespace CircleERP.Application.Addresses.GetAddressById;

internal sealed class GetAddressByIdQueryHandler(IAddressRepository addresses)
    : IQueryHandler<GetAddressByIdQuery, AddressResponse>
{
    public async Task<Result<AddressResponse>> Handle(
        GetAddressByIdQuery query,
        CancellationToken cancellationToken)
    {
        var address = await addresses.GetByIdAsync(query.Id, cancellationToken);

        if (address is null)
            return Result.Fail<AddressResponse>(AddressErrors.NotFound(query.Id));

        return Result.Ok(address.ToResponse());
    }
}
