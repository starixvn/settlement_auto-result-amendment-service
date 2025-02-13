using Fanex.Data.Repository;
using Microsoft.Extensions.Caching.Memory;

namespace ResultAmendment.Features.GetMainResource;

public interface IGetMainResourceHandler
{
    Task<List<string>> HandlerAsync(int sportType);
}

public class GetMainResourceHandler : IGetMainResourceHandler
{
    private readonly IDynamicRepository repository;
    private readonly IMemoryCache memoryCache;

    public GetMainResourceHandler(IDynamicRepository repository, IMemoryCache memoryCache)
    {
        this.repository = repository;
        this.memoryCache = memoryCache;
    }

    public async Task<List<string>> HandlerAsync(int sportType)
    {
        var cachedValue = await memoryCache.GetOrCreateAsync(
            $"MainResource_{sportType}",
            async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return await GetMainResource(sportType);
            });

        return cachedValue ?? [];
    }

    private async Task<List<string>> GetMainResource(int sportType)
    {
        var criteria = new GetMainResourceCriteria(sportType);
        var mainResource = await repository.FetchAsync<Resource>(criteria);
        return mainResource.Select(x => x.ResourceId).ToList();
    }
}
