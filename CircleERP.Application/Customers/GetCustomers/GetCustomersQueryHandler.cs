using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Domain.Addresses;
using CircleERP.Domain.Customers;
using FluentResults;

namespace CircleERP.Application.Customers.GetCustomers;

internal sealed class GetCustomersQueryHandler(
    ICustomerRepository customers,
    IAddressRepository addresses)
    : IQueryHandler<GetCustomersQuery, IReadOnlyList<CustomerResponse>>
{
    public async Task<Result<IReadOnlyList<CustomerResponse>>> Handle(
        GetCustomersQuery query,
        CancellationToken cancellationToken)
    {
        var result = await customers.GetAllAsync(cancellationToken);

        // Os enderecos vem em uma consulta so. Resolver um por cliente dentro do
        // laco seria o classico N+1.
        var addressIds = result
            .Where(customer => customer.AddressId.HasValue)
            .Select(customer => customer.AddressId!.Value)
            .Distinct()
            .ToArray();

        var addressesById = (await addresses.GetByIdsAsync(addressIds, cancellationToken))
            .ToDictionary(address => address.Id);

        return Result.Ok<IReadOnlyList<CustomerResponse>>(
        [
            .. result.Select(customer => customer.ToResponse(
                customer.AddressId is { } id && addressesById.TryGetValue(id, out var address)
                    ? address
                    : null))
        ]);
    }
}
