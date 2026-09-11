using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Application.Abstractions.Persistence;
using CircleERP.Application.Addresses;
using CircleERP.Domain.Addresses;
using CircleERP.Domain.Customers;
using FluentResults;

namespace CircleERP.Application.Customers.ChangeCustomer;

internal sealed class ChangeCustomerCommandHandler(
    ICustomerRepository customers,
    IAddressRepository addresses,
    IUnitOfWork unitOfWork) : ICommandHandler<ChangeCustomerCommand>
{
    public async Task<Result> Handle(
        ChangeCustomerCommand command,
        CancellationToken cancellationToken)
    {
        var customer = await customers.GetByIdAsync(command.Id, cancellationToken);

        if (customer is null)
            return Result.Fail(CustomerErrors.NotFound(command.Id));

        var (name, document) = command.Fields.ToValueObjects();

        // Ignora o proprio cliente: manter o mesmo documento nao e duplicata.
        if (await customers.ExistsWithDocumentAsync(document, command.Id, cancellationToken))
            return Result.Fail(
                CustomerErrors.DocumentAlreadyRegistered(document.ToFormattedString()));

        if (command.Fields.AddressId is { } addressId
            && !await addresses.ExistsAsync(addressId, cancellationToken))
        {
            return Result.Fail(AddressErrors.NotFound(addressId));
        }

        customer.Change(name, document);

        if (command.Fields.AddressId is { } linkedAddressId)
            customer.LinkAddress(linkedAddressId);
        else
            customer.UnlinkAddress();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
