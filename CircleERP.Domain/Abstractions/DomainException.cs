namespace CircleERP.Domain.Abstractions;

/// <summary>
/// Invariante de negocio violada. Sinaliza bug de uso do dominio ou entrada
/// que passou pela validacao da borda -- nao e o mesmo que erro de aplicacao
/// ("moeda nao encontrada"), que a Application expressa como resultado.
/// </summary>
public class DomainException(string message) : Exception(message);
