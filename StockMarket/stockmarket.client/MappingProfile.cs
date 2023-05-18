using AutoMapper;
using StockMarket.Client.ViewModels;
using StockMarket.Domain;

namespace StockMarket.Client;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Stock, StockViewModel> ();
        CreateMap<Quote, StockViewModel> ();
        CreateMap<Quote, QuoteViewModel> ();
    }
}