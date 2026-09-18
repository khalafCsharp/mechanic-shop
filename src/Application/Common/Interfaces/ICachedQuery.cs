using MediatR;

namespace Application.Common.Interfaces
{
    public interface ICachedQuery
    {
        string CacheKey {get;}
        string[] Tags {get;}
        TimeSpan ExpiresIn {get;}
    }

    public interface ICachedQuery<TResponse> : ICachedQuery, IRequest<TResponse>;
}