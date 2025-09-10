using CircleERP.Model.Mapper;
using CircleERP.Model.Data;
using CircleERP.Model.Data.Dto.Currency;
using CircleERP.Model;
using FluentResults;
using System.Collections.Generic;
using System.Linq;
using System;

namespace CircleERP.Model.Services;

public class CurrencyService
{
    private readonly AppDbContext _context;
    private readonly CurrencyMapper _mapper;

    public CurrencyService(AppDbContext appDbContext)
    {
        _context = appDbContext;
        _mapper = new CurrencyMapper();
    }

    public List<ReadCurrencyDto> GetAllCurrencys()
    {
        List<Currency> currencies = _context.Currencys.ToList();
        
        return currencies?.Select(currency => _mapper.MapToReadDto(currency)).ToList();
    }

    public Result UpdateCurrency(int id, UpdateCurrencyDto currencyDto)
    {
        Currency currency = _context.Currencys.FirstOrDefault(currency => currency.Id == id);

        if (currency == null)
            return Result.Fail("Moeda não encontrada.");

        currency.Rating = currencyDto.Rating;
        currency.Description = currencyDto.Description;

        _context.Update(currency);
        _context.SaveChanges();
        return Result.Ok();
    }

    public Result UpdateCurrency(string code, UpdateCurrencyDto currencyDto)
    {
        Currency currency = _context.Currencys.FirstOrDefault(currency => currency.Code == code);

        if (currency == null)
            return Result.Fail("Moeda não encontrada.");

        currency.Rating = currencyDto.Rating;
        currency.Description = currencyDto.Description;

        _context.Update(currency);
        _context.SaveChanges();
        return Result.Ok();
    }

    public Result<Currency> PostCurrency(CreateCurrencyDto createCurrencyDto)
    {
        if (VerifyIfCurrencyExists(createCurrencyDto.Code))
            return Result.Fail<Currency>("Moeda já cadastrada no sistema.");

        Currency currency = _mapper.MapToCurrency(createCurrencyDto);
        _context.Add(currency);
        _context.SaveChanges();

        return Result.Ok(currency);
    }

    public Result DeleteCurrency(int id)
    {
        if (!VerifyIfCurrencyExists(id))
            return Result.Fail("Moeda não existe no sistema.");
        _context.Remove(_context.Currencys.FirstOrDefault(c => c.Id == id));
        _context.SaveChanges();

        return Result.Ok();
    }

    public Result DeleteCurrency(string code)
    {
        if (!VerifyIfCurrencyExists(code))
            return Result.Fail("Moeda não existe no sistema.");
        _context.Remove(_context.Currencys.FirstOrDefault(c => c.Code == code));
        _context.SaveChanges();

        return Result.Ok();
    }

    private bool VerifyIfCurrencyExists(string code)
    {
        Currency currency = _context.Currencys.FirstOrDefault(currency => currency.Code.ToUpper() == code.ToUpper());
        return currency != null;
    }

    private bool VerifyIfCurrencyExists(int id)
    {
        Currency currency = _context.Currencys.FirstOrDefault(currency => currency.Id == id);
        return currency != null;
    }
}
