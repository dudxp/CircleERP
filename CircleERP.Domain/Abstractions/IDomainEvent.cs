namespace CircleERP.Domain.Abstractions;

/// <summary>
/// Algo relevante que aconteceu no dominio e que outras partes do sistema
/// podem querer reagir (estoque, notificacao, integracao).
/// </summary>
/// <remarks>
/// Deliberadamente sem heranca de nenhum tipo de biblioteca: o dominio nao
/// conhece MediatR. A adaptacao para <c>INotification</c> acontece na camada
/// de Application.
/// </remarks>
public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}
