using Fanex.Data.Repository;
using Microsoft.Extensions.Caching.Memory;

namespace ResultAmendment.Features.GetMainResource;

public interface IGetMainResourceHandler
{
    Task<List<string>> HandlerAsync(int sportType);
}

public class GetMainResourceHandler(IDynamicRepository repository, IMemoryCache memoryCache) : IGetMainResourceHandler
{
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