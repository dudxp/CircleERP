using CircleERP.Domain.Abstractions;

namespace CircleERP.Domain.Addresses;

/// <summary>
/// Endereco. Raiz de agregado propria: tem cadastro e tela proprios, e e
/// vinculado ao cliente por identidade.
/// </summary>
public sealed class Address : Entity<int>, IAggregateRoot
{
    public const int StreetMaxLength = 150;
    public const int NumberMaxLength = 20;
    public const int ComplementMaxLength = 60;
    public const int DistrictMaxLength = 80;
    public const int CityMaxLength = 80;

    /// <summary>Exigido pelo EF Core.</summary>
    private Address()
    {
        ZipCode = null!;
        Street = null!;
        Number = null!;
        District = null!;
        City = null!;
        State = null!;
    }

    private Address(
        ZipCode zipCode,
        AddressText street,
        AddressText number,
        AddressText? complement,
        AddressText district,
        AddressText city,
        StateCode state)
    {
        ZipCode = zipCode;
        Street = street;
        Number = number;
        Complement = complement;
        District = district;
        City = city;
        State = state;
    }

    public ZipCode ZipCode { get; private set; }

    public AddressText Street { get; private set; }

    /// <summary>Texto, e nao numero: "S/N" e "123A" sao numeros validos.</summary>
    public AddressText Number { get; private set; }

    public AddressText? Complement { get; private set; }

    public AddressText District { get; private set; }

    public AddressText City { get; private set; }

    public StateCode State { get; private set; }

    public static Address Register(
        ZipCode zipCode,
        AddressText street,
        AddressText number,
        AddressText? complement,
        AddressText district,
        AddressText city,
        StateCode state) =>
        new(zipCode, street, number, complement, district, city, state);

    public void Change(
        ZipCode zipCode,
        AddressText street,
        AddressText number,
        AddressText? complement,
        AddressText district,
        AddressText city,
        StateCode state)
    {
        ZipCode = zipCode;
        Street = street;
        Number = number;
        Complement = complement;
        District = district;
        City = city;
        State = state;
    }

    /// <summary>Endereco em uma linha, para listagem e para o seletor.</summary>
    public string ToSingleLine()
    {
        var complement = Complement is null ? string.Empty : $" - {Complement}";

        return $"{Street}, {Number}{complement} - {District}, {City}/{State}";
    }
}
