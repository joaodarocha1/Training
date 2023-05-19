using AutoMapper;
using Moq;
using Serilog;
using StockMarket.Service.Bloomberg.Publisher;
using StockMarket.Service.Bloomberg;
using StockMarket.Client.Utils;

namespace StockMarket.Client.Test;

public class BaseViewModelTests
{
    public IMapper Mapper { get; set; }
    public Mock<ILogger> Logger { get; }
    protected MarketDataService DataService { get; }
    protected Mock<IRandomPublisher> Publisher { get; }
    public Mock<IDispatcherService> DispatcherServiceMock { get; }

    public BaseViewModelTests()
    {
        var configuration = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
        Logger = new Mock<ILogger>();
        Mapper = configuration.CreateMapper();
        Publisher = new Mock<IRandomPublisher>();
        DataService = new MarketDataService(Publisher.Object, Logger.Object);
        DispatcherServiceMock = new Mock<IDispatcherService>();
        DispatcherServiceMock.Setup(m => m.Invoke(It.IsAny<Action>())).Callback<Action>(a => a());
    }
}