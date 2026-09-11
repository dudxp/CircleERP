using CircleERP.Application.Addresses;
using CircleERP.Domain.Addresses;
using CircleERP.Domain.Customers;

namespace CircleERP.Application.Customers;

internal static class CustomerMappings
{
    internal static CustomerResponse ToResponse(this Customer customer, Address? address) =>
        new(
            customer.Id,
            customer.Name.Value,
            customer.Document.Value,
            customer.Document.ToFormattedString(),
            customer.Type.ToString(),
            customer.IsActive,
            customer.AddressId,
            address?.ToResponse());
}
