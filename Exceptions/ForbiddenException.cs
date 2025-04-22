namespace TodoApi.Exceptions;
public class ForbiddenException(string traceId, string message) : ServiceException(statusCode,traceId,message){
    private const int statusCode = 403;
}