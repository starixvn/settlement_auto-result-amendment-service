using Fanex.Settlement.MessageBridge.ResultAmendment;
using ResultAmendment.Models;

public interface ISetPositionStrategy
{
    (ResultItem wrongResult, ResultItem actualResult) Generate(IList<ResultChanged> resultChanged, IList<ResourceItem> resourceItem);
}
