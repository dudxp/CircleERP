namespace CircleERP.Domain.Abstractions;

/// <summary>
/// Marca a unica entidade de um agregado que pode ser referenciada de fora
/// dele. Repositorios existem por raiz de agregado, nunca por entidade interna.
/// </summary>
public interface IAggregateRoot;
