using CircleERP.Application.Abstractions.Messaging;

namespace CircleERP.Application.Products.ChangeProduct;

public sealed record ChangeProductCommand(int Id, ProductFields Fields) : ICommand;
