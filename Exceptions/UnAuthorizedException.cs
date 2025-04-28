namespace TodoApi.Exceptions;
public class UnAuthorizedException:ServiceException{
    private const int statusCode = 401;
    public UnAuthorizedException(string message):base(statusCode,message){}
}