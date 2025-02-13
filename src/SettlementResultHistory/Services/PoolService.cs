using AutoResultAmendment.Models;

namespace AutoResultAmendment.Services;

public interface PoolService
{
    Task<List<ResultHistorySettled>> PeekResultHistory();

    void Remove(ResultHistorySettled item);
}

public class PoolServiceImp(SettlementResultHistoryRepository resultHistoryRepository) : PoolService
{
    private readonly List<ResultHistorySettled> poolOfResultHistory = new();
    public List<ResultHistorySettled> PoolOfResultHistory => poolOfResultHistory;

    private readonly object lockObject = new();

    public async Task<List<ResultHistorySettled>> PeekResultHistory()
    {
        var result = await resultHistoryRepository.GetResultHistoryByEnv();

        lock (lockObject)
        {
            var newResult = result.Except(poolOfResultHistory).ToList();
            poolOfResultHistory.AddRange(newResult);
            return newResult;
        }

        return result;
    }

    public void Remove(ResultHistorySettled item)
    {
        lock (lockObject)
        {
            poolOfResultHistory.RemoveAll(m => m.SessionId == item.SessionId && m.DbServer == item.DbServer);
        }
    }
}