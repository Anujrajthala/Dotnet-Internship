namespace TodoApi.DTOs;
public class ResponseDTO<T>{
    public bool Success{get; set;}
    public string Message{get; set;}= string.Empty;
    public T? Data{get; set;}
    public ResponseDTO(bool success, string message,T data){
        Success = success;
        Message = message;
        Data = data;
    }

    public ResponseDTO(bool success,string message){
        Success = success;
        Message= message;
        Data = default;
    }
}