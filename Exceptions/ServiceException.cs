namespace TodoApi.Exceptions;
public class ServiceException:Exception{
    public int StatusCode;
    public string TraceId;
    public ServiceException(int statusCode, string traceId, string message):base(message){
        StatusCode = statusCode;
        TraceId = traceId;
    }
}