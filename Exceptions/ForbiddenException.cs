namespace TodoApi.Exceptions;
public class ForbiddenException(string message) : ServiceException(statusCode,message){
    private const int statusCode = 403;
}