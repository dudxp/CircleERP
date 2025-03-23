using AutoMapper;
using CircleERP.Model.ServiceModel.Data.Dto.Currency;
using CircleERP.Model.ServiceModel.Model;

namespace CircleERP.Model;

class CurrencyProfile : Profile
{
    public CurrencyProfile()
    {
        CreateMap<CreateCurrencyDto, Currency>();
        CreateMap<Currency, ReadCurrencyDto>();
        CreateMap<UpdateCurrencyDto, Currency>();
    }
}
