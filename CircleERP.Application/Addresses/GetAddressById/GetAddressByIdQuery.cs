using CircleERP.Application.Abstractions.Messaging;

namespace CircleERP.Application.Addresses.GetAddressById;

public sealed record GetAddressByIdQuery(int Id) : IQuery<AddressResponse>;
