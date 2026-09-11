using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Domain.Addresses;
using CircleERP.Domain.Customers;
using FluentResults;

namespace CircleERP.Application.Customers.GetCustomerById;

internal sealed class GetCustomerByIdQueryHandler(
    ICustomerRepository customers,
    IAddressRepository addresses)
    : IQueryHandler<GetCustomerByIdQuery, CustomerResponse>
{
    public async Task<Result<CustomerResponse>> Handle(
        GetCustomerByIdQuery query,
        CancellationToken cancellationToken)
    {
        var customer = await customers.GetByIdAsync(query.Id, cancellationToken);

        if (customer is null)
            return Result.Fail<CustomerResponse>(CustomerErrors.NotFound(query.Id));

        var address = customer.AddressId is { } addressId
            ? await addresses.GetByIdAsync(addressId, cancellationToken)
            : null;

        return Result.Ok(customer.ToResponse(address));
    }
}
