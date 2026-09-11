using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Domain.Addresses;
using FluentResults;

namespace CircleERP.Application.Addresses.GetAddresses;

internal sealed class GetAddressesQueryHandler(IAddressRepository addresses)
    : IQueryHandler<GetAddressesQuery, IReadOnlyList<AddressResponse>>
{
    public async Task<Result<IReadOnlyList<AddressResponse>>> Handle(
        GetAddressesQuery query,
        CancellationToken cancellationToken)
    {
        var result = await addresses.GetAllAsync(cancellationToken);

        return Result.Ok<IReadOnlyList<AddressResponse>>(
            [.. result.Select(address => address.ToResponse())]);
    }
}
