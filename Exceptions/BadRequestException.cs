namespace TodoApi.Exceptions;
public class BadRequestException: ServiceException{
    private const int statusCode = 400;
    public BadRequestException(string message):base(statusCode,message){
    
    }
}
