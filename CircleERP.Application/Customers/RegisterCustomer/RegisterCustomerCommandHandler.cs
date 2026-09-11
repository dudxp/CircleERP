using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Application.Abstractions.Persistence;
using CircleERP.Application.Addresses;
using CircleERP.Domain.Addresses;
using CircleERP.Domain.Customers;
using FluentResults;

namespace CircleERP.Application.Customers.RegisterCustomer;

internal sealed class RegisterCustomerCommandHandler(
    ICustomerRepository customers,
    IAddressRepository addresses,
    IUnitOfWork unitOfWork) : ICommandHandler<RegisterCustomerCommand, int>
{
    public async Task<Result<int>> Handle(
        RegisterCustomerCommand command,
        CancellationToken cancellationToken)
    {
        var (name, document) = command.Fields.ToValueObjects();

        if (await customers.ExistsWithDocumentAsync(document, cancellationToken))
            return Result.Fail<int>(
                CustomerErrors.DocumentAlreadyRegistered(document.ToFormattedString()));

        // O endereco e outro agregado: existir e responsabilidade do caso de uso.
        if (command.Fields.AddressId is { } addressId
            && !await addresses.ExistsAsync(addressId, cancellationToken))
        {
            return Result.Fail<int>(AddressErrors.NotFound(addressId));
        }

        var customer = Customer.Register(name, document, command.Fields.AddressId);

        customers.Add(customer);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(customer.Id);
    }
}
