using MassTransit;
using Microsoft.Extensions.Logging;
using ResultAmendment.Contracts;
using ResultAmendment.Features.AddSettlementResultHistory;

namespace ResultAmendment.Consumers;

public sealed class ResultAmendedConsumer : IConsumer<ResultAmended>
{
    private readonly ILogger<ResultAmendedConsumer> _logger;
    private readonly SettlementResultHistoryHandler _handler;

    public ResultAmendedConsumer(ILogger<ResultAmendedConsumer> logger, SettlementResultHistoryHandler handler)
    {
        _logger = logger;
        _handler = handler;
    }

    public async Task Consume(ConsumeContext<ResultAmended> context)
    {
        _logger.LogInformation("ResultAmendedConsumer: {MatchId}", context.Message.MatchId);

        await _handler.HandleAsync(context.Message);
    }


    private static short GetReSettleActionType(string beforeAction, string currentAction)
    {
        var before = beforeAction.ToUpperInvariant();
        var current = currentAction.ToUpperInvariant();

        return (before, current) switch
        {
            var (b, c) when b.Contains(ActionType.RefundAll) && c.Contains(ActionType.CalculateAll) => ReSettleType.RefundToCalc,
            var (b, c) when b.Contains(ActionType.CalculateAll) && c.Contains(ActionType.RefundAll) => ReSettleType.CalcToRefund,
            var (b, c) when b.Contains(ActionType.CalculateAll) && c.Contains(ActionType.CalculateAll) => ReSettleType.ReCalc,
            _ => ReSettleType.UnKnow
        };
    }

}