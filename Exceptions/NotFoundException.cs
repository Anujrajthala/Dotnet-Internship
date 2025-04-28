namespace TodoApi.Exceptions;
public class NotFoundException(string message) : ServiceException(statusCode,message){
    private const int statusCode = 404;
}