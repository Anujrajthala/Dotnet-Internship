namespace TodoApi.Exceptions;
public class BadRequestException: ServiceException{
    public List<string> Errors{get;}
    private const int statusCode = 400;
    public BadRequestException(string traceId,string message,List<string> errors):base(statusCode,message,traceId){
        Errors = errors;
    }
}

public class BadRequestExceptionII : Exception
{
    public IEnumerable<string> Errors { get; }

    public BadRequestExceptionII(string message) : base(message)
    {
        Errors = new List<string> { message };
    }

    public BadRequestExceptionII(string message, IEnumerable<string> errors) : base(message)
    {
        Errors = errors;
    }
}