namespace TodoApi.DTOs.TodoDTO;
public class TodoUpdateResponseDTO{
    public bool Success{get; set;}
    public string Message{get; set;}

    public TodoUpdateResponseDTO(bool success, string message){
        Success = success;
        Message = message;
    }
}