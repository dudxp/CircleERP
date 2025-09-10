using CircleERP.Model.Data.Dto.Currency;
using CircleERP.Model;
using System;

namespace CircleERP.Model.Mapper;

class CurrencyMapper
{
    public ReadCurrencyDto MapToReadDto(Currency currency)
    {
        return new ReadCurrencyDto
        {
            Code = currency.Code,
            Id = currency.Id,
            Rating = currency.Rating,
            Description = currency.Description
        };
    }

    public Currency MapToCurrency(CreateCurrencyDto createCurrencyDto)
    {
        return new Currency
        {
            Code = createCurrencyDto.Code,
            Description = createCurrencyDto.Description,
            Rating = createCurrencyDto.Rating
        };
    }
}
