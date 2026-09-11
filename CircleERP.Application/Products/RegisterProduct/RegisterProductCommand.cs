using CircleERP.Application.Abstractions.Messaging;

namespace CircleERP.Application.Products.RegisterProduct;

public sealed record RegisterProductCommand(ProductFields Fields) : ICommand<int>;
