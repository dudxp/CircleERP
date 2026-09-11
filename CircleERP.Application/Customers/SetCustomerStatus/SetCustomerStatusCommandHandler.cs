using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Application.Abstractions.Persistence;
using CircleERP.Application.Abstractions.Time;
using CircleERP.Domain.Customers;
using FluentResults;

namespace CircleERP.Application.Customers.SetCustomerStatus;

internal sealed class SetCustomerStatusCommandHandler(
    ICustomerRepository customers,
    IDateTimeProvider dateTime,
    IUnitOfWork unitOfWork) : ICommandHandler<SetCustomerStatusCommand>
{
    public async Task<Result> Handle(
        SetCustomerStatusCommand command,
        CancellationToken cancellationToken)
    {
        var customer = await customers.GetByIdAsync(command.Id, cancellationToken);

        if (customer is null)
            return Result.Fail(CustomerErrors.NotFound(command.Id));

        if (command.Active)
            customer.Activate();
        else
            customer.Deactivate(dateTime.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
