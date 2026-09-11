using CircleERP.Domain.Abstractions;
using CircleERP.Domain.Customers.Events;

namespace CircleERP.Domain.Customers;

/// <summary>
/// Cliente. Raiz de agregado.
/// </summary>
/// <remarks>
/// O endereco e referenciado por identidade (<see cref="AddressId"/>), e nao
/// embutido: endereco tem cadastro e tela proprios, e o mesmo endereco pode
/// existir independente de haver cliente vinculado a ele.
/// </remarks>
public sealed class Customer : Entity<int>, IAggregateRoot
{
    /// <summary>Exigido pelo EF Core.</summary>
    private Customer()
    {
        Name = null!;
        Document = null!;
    }

    private Customer(CustomerName name, Document document, int? addressId)
    {
        Name = name;
        SetDocument(document);
        AddressId = addressId;
        IsActive = true;
    }

    public CustomerName Name { get; private set; }

    public Document Document { get; private set; }

    /// <summary>
    /// Acompanha o documento, e nunca e informado em paralelo: assim nao existe
    /// caminho para um cliente marcado como pessoa juridica portando um CPF.
    /// </summary>
    /// <remarks>
    /// Tem setter privado apenas para o EF conseguir persistir a coluna; o
    /// unico ponto do codigo que escreve nele e <see cref="SetDocument"/>.
    /// </remarks>
    public CustomerType Type { get; private set; }

    /// <summary>Endereco vinculado. Ausente enquanto nenhum foi informado.</summary>
    public int? AddressId { get; private set; }

    /// <summary>
    /// Cliente inativo nao pode receber pedidos novos, mas continua existindo:
    /// os pedidos antigos dele precisam seguir legiveis.
    /// </summary>
    public bool IsActive { get; private set; }

    public static Customer Register(CustomerName name, Document document, int? addressId = null)
    {
        var customer = new Customer(name, document, addressId);

        customer.Raise(new CustomerRegistered(document.Value, DateTime.UtcNow));

        return customer;
    }

    public void Change(CustomerName name, Document document)
    {
        Name = name;
        SetDocument(document);
    }

    /// <summary>Mantem documento e tipo sempre coerentes.</summary>
    private void SetDocument(Document document)
    {
        Document = document;
        Type = document.Type;
    }

    public void LinkAddress(int addressId) => AddressId = addressId;

    public void UnlinkAddress() => AddressId = null;

    public void Deactivate(DateTime occurredOnUtc)
    {
        if (!IsActive)
            throw new DomainException("O cliente ja esta inativo.");

        IsActive = false;

        Raise(new CustomerDeactivated(Id, occurredOnUtc));
    }

    public void Activate()
    {
        if (IsActive)
            throw new DomainException("O cliente ja esta ativo.");

        IsActive = true;
    }
}
