using Fanex.Settlement.MessageBridge.ResultAmendment;
using ResultAmendment.Models;

namespace ResultAmendment.Features.SetResultsStrategy;

public static class ResultItemHelper
{
    public static ResultItem CreateResultItem(ResultChanged resultChanged, ResourceItem resourceItem, string resourceId, string betTypeId, int setX, int pointY, bool includeOrdinal)
    {
        return new ResultItem
        {
            ResourceId = resourceId,
            Home = ResultInfoHelpers.NormalizeHomeValue(resultChanged.HomeValue),
            Away = resultChanged.AwayValue,
            RefBetTypeId = betTypeId,
            TeamIds = ResultInfoHelpers.GetTeamIds(resultChanged.HomeValue, resourceItem),
            SetX = $"{setX}",
            PointY = $"{pointY}",
            Ordinal = includeOrdinal ? $"K{pointY}" : null
        };
    }

    public static ResultItem CreateResultItem(ResourceItem resourceItem, string resourceId, string betTypeId, int setX, int pointY, bool includeOrdinal)
    {
        return new ResultItem
        {
            ResourceId = resourceId,
            Home = ResultInfoHelpers.NormalizeHomeValue(resourceItem.HomeValue),
            Away = resourceItem.AwayValue,
            RefBetTypeId = betTypeId,
            TeamIds = ResultInfoHelpers.GetTeamIds(resourceItem.HomeValue, resourceItem),
            SetX = $"{setX}",
            PointY = $"{pointY}",
            Ordinal = includeOrdinal ? $"K{pointY}" : null
        };
    }
}