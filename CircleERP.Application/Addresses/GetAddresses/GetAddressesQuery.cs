using CircleERP.Application.Abstractions.Messaging;

namespace CircleERP.Application.Addresses.GetAddresses;

public sealed record GetAddressesQuery : IQuery<IReadOnlyList<AddressResponse>>;
