using Fanex.Data.Repository;
using Fanex.Settlement.Converter.Services;
using ResultAmendment.Contracts;
using ResultAmendment.Features.GetMainResource;
using ResultAmendment.Features.SetResultsStrategy;
using ResultAmendment.Models;

namespace ResultAmendment.Features.AddSettlementResultHistory;

public class SettlementResultHistoryHandler
{
    private readonly IDynamicRepository repository;
    private readonly XmlConvertService xmlConvertService;
    private readonly GetMainResourceHandler getMainResourceHandler;

    public SettlementResultHistoryHandler(IDynamicRepository repository, XmlConvertService xmlConvertService, GetMainResourceHandler getMainResourceHandler)
    {
        this.repository = repository;
        this.xmlConvertService = xmlConvertService;
        this.getMainResourceHandler = getMainResourceHandler;
    }

    public async Task HandleAsync(ResultAmended resultAmended)
    {
        var mainResources = await getMainResourceHandler.HandlerAsync(resultAmended.SportType);
        var wrongResults = new List<ResultItem>();
        var actualResults = new List<ResultItem>();

        foreach (var item in resultAmended.ResultsChanged)
        {
            var resourceItem = resultAmended.SelectedResources.Find(x => x.ResourceId == item.ResourceId);

            var resultStrategy = ResultStrategyFactory.GetStrategy(item.ResourceId, resourceItem.GroupBetType, mainResources);
            var message = new Message(resultStrategy);
            message.SetResults(item, resourceItem);

            Console.WriteLine(message.WrongResult);
            Console.WriteLine(message.ActualResult);
        }

        //var resultHistoryItem = new SettlementResultHistoryItem
        //{
        //    //SeqNo = seqNo,
        //    WrongResult = xmlConvertService.Serialize(new ResultInfo { ResultItems = wrongResults }),
        //    ActualResult = xmlConvertService.Serialize(new ResultInfo { ResultItems = actualResults }),
        //    ExtraInfo = CreateExtraInfo(resultAmended.ResultsChanged),
        //    //ActionType = actionType
        //};

        //var criteria = new ResultHistoryInsCriteria
        //{
        //    MatchId = resultAmended.MatchId,
        //    SessionID = resultAmended.BroadcastSessionId,
        //    SportType = resultAmended.SportType,
        //    //ResourceInfo = JsonSerializer.Serialize([resultHistoryItem])
        //};
    }

    public string CreateExtraInfo(List<ResultChanged> resultsChanged)
    {
        var extraInfoItems = resultsChanged.Select(x => new ExtraInfoItem
        {
            ResourceId = x.ResourceId,
            CalcDateTime = x.LogDate.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            ReCalcDateTime = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
        }).ToList();

        var extraInfo = new ExtraInfo { ExtraInfoItems = extraInfoItems };

        return xmlConvertService.Serialize(extraInfo);
    }
}
