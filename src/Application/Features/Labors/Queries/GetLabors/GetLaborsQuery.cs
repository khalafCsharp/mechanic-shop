using Application.Common.Interfaces;
using Application.Features.Labors.Dtos;
using Domain.Common.Results;

namespace Application.Features.Labors.Queries.GetLabors
{
    public class GetLaborsQuery : ICachedQuery<Result<List<LaborDto>>>
    {
        public string CacheKey => "labors";

        public string[] Tags => ["labors"];

        public TimeSpan ExpiresIn => TimeSpan.FromMinutes(10);
    }
}