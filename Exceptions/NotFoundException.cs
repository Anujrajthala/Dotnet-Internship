namespace TodoApi.Exceptions;
public class NotFoundException(string traceId, string message) : ServiceException(statusCode,traceId,message){
    private const int statusCode = 404;
}