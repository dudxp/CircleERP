namespace CircleERP.Application.Abstractions.Time;

/// <summary>
/// Fonte da hora atual.
/// </summary>
/// <remarks>
/// Os metodos do dominio recebem o instante como parametro em vez de chamar
/// <c>DateTime.UtcNow</c> por dentro, justamente para serem testaveis. Este
/// contrato e quem fornece esse instante nos casos de uso.
/// </remarks>
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
