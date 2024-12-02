using ResultAmendment.Models;

namespace ResultAmendment.Features.SetResultsStrategy;

public class Message
{
    private readonly ISetResultStrategy _resultStrategy;

    public Message(ISetResultStrategy resultStrategy)
    {
        _resultStrategy = resultStrategy;
    }

    public ResultItem WrongResult { get; protected set; }
    public ResultItem ActualResult { get; protected set; }

    public void SetResults(ResultChanged resultChanged, ResourceItem resourceItem)
    {
        var (wrongResult, actualResult) = _resultStrategy.Generate(resultChanged, resourceItem);

        WrongResult = wrongResult;
        ActualResult = actualResult;
    }
}