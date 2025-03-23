using AutoMapper;
using CircleERP.Model.ServiceModel.Data;
using CircleERP.Model.ServiceModel.Data.Dto.Currency;
using CircleERP.Model.ServiceModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CircleERP.Model.Model.Services;

public class CurrencyService
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;

    public CurrencyService(AppDbContext appDbContext, IMapper mapper)
    {
        _context = appDbContext;
        _mapper = mapper;
    }

    public List<ReadCurrencyDto> GetAllCurrency()
    {
        List<Currency> currencies = _context.Currencys.ToList();
        
        if (currencies == null)
        {
            return null;
        }
        else
        {
            List<ReadCurrencyDto> readDto = _mapper.Map<List<ReadCurrencyDto>>(currencies);
            return readDto;
        }
    }
}
