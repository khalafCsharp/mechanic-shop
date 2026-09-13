using Domain.Common.Results.Errors;

namespace Domain.Common.Results.Interfaces
{
    public interface IResult
    {
        List<Error> Errors {get;}
        bool IsSuccess {get;}
    }
    public interface IResult<out TValue> : IResult
    {
        TValue Value {get;}
    }
}