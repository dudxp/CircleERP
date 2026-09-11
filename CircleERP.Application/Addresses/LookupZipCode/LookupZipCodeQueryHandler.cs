using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Application.Abstractions.ZipCodes;
using CircleERP.Domain.Addresses;
using FluentResults;

namespace CircleERP.Application.Addresses.LookupZipCode;

internal sealed class LookupZipCodeQueryHandler(IZipCodeLookup lookup)
    : IQueryHandler<LookupZipCodeQuery, ZipCodeLookupResult>
{
    public async Task<Result<ZipCodeLookupResult>> Handle(
        LookupZipCodeQuery query,
        CancellationToken cancellationToken)
    {
        // O value object valida o formato antes de qualquer chamada externa:
        // nao vale gastar uma requisicao para descobrir que "123" nao e um CEP.
        var zipCode = ZipCode.Create(query.ZipCode);

        try
        {
            var result = await lookup.FindAsync(zipCode, cancellationToken);

            return result is null
                ? Result.Fail<ZipCodeLookupResult>(AddressErrors.ZipCodeNotFound(zipCode.ToFormattedString()))
                : Result.Ok(result);
        }
        catch (ZipCodeLookupUnavailableException exception)
        {
            // Consulta indisponivel nao impede o cadastro: a tela cai para o
            // preenchimento manual.
            return Result.Fail<ZipCodeLookupResult>(AddressErrors.LookupUnavailable(exception.Message));
        }
    }
}
