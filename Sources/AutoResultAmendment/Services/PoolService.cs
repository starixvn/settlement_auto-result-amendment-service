using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoResultAmendment.Models;
using Microsoft.Extensions.FileSystemGlobbing;

namespace AutoResultAmendment.Services;

public interface PoolService
{
    Task<List<ResultHistorySettled>> PeekResultHistory();

    void Remove(ResultHistorySettled item);
}

public class PoolServiceImp : PoolService
{
    private readonly List<ResultHistorySettled> poolOfResultHistory = new();
    public List<ResultHistorySettled> PoolOfResultHistory => poolOfResultHistory;

    private readonly object lockObject;

    private readonly SettlementResultHistoryRepository resultHistoryRepository;

    public PoolServiceImp(SettlementResultHistoryRepository resultHistoryRepository)
    {
        this.resultHistoryRepository = resultHistoryRepository;
        lockObject = new object();
    }

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