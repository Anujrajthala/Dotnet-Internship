namespace TodoApi.DTOs.TodoDTO;
public class TodoDeleteResponseDTO{
    public bool Success{get; set;}
    public string Message{get; set;}

    public TodoDeleteResponseDTO(bool success, string message){
        Success = success;
        Message = message;
    }
}