using CircleERP.Application.Abstractions.Messaging;
using CircleERP.Application.Abstractions.ZipCodes;

namespace CircleERP.Application.Addresses.LookupZipCode;

public sealed record LookupZipCodeQuery(string ZipCode) : IQuery<ZipCodeLookupResult>;
