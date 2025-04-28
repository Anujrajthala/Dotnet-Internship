namespace TodoApi.Exceptions;
public class ServiceException:Exception{
    public int StatusCode{get; set;}
    public ServiceException(int statusCode, string message):base(message){
        StatusCode = statusCode;
    }
}