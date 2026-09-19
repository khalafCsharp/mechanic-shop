using System.Buffers;
using Application.Common.Interfaces;
using Domain.Common.Results.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors
{
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;
        public readonly HybridCache _cache;

        public CachingBehavior(ILogger<CachingBehavior<TRequest, TResponse>> logger, HybridCache cache)
        {
            this._cache = cache;
            this._logger = logger;

        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request is not ICachedQuery cachedRequest)
            {
                return await next(cancellationToken);
            }
            _logger.LogInformation("Checking cache for {RequestName}", typeof(TRequest).Name);

            var cacheResult = await _cache.GetOrCreateAsync(
                cachedRequest.CacheKey,
                _ => new ValueTask<TResponse>(null!),
                new HybridCacheEntryOptions()
                {
                    Flags = HybridCacheEntryFlags.DisableUnderlyingData
                },
                cancellationToken: cancellationToken);

            if (cacheResult is null)
            {
                cacheResult = await next(cancellationToken);

                if (cacheResult is IResult returnedResult && returnedResult.IsSuccess)
                {
                    _logger.LogInformation("Caching result for {RequestName}", typeof(TRequest).Name);

                    await _cache.SetAsync(cachedRequest.CacheKey, cacheResult, new HybridCacheEntryOptions
                    {
                        Expiration = cachedRequest.ExpiresIn
                    }, cachedRequest.Tags, cancellationToken);
                }
            }
            return cacheResult;
        }
    }
}