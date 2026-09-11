using CircleERP.Domain.Addresses;

namespace CircleERP.Application.Addresses;

internal static class AddressMappings
{
    internal static AddressResponse ToResponse(this Address address) =>
        new(
            address.Id,
            address.ZipCode.Value,
            address.ZipCode.ToFormattedString(),
            address.Street.Value,
            address.Number.Value,
            address.Complement?.Value,
            address.District.Value,
            address.City.Value,
            address.State.Value,
            address.ToSingleLine());
}
