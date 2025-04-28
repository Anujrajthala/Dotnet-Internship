namespace TodoApi.DTOs.TodoDTO;
public class TodoListResponseDTO{
    public List<TodoResponseDTO>? Response {get; set;}
    public string Message{get; set;}

    public TodoListResponseDTO(List<TodoResponseDTO> dtos,string message){
        Response = dtos;
        Message = message;
    }
    public TodoListResponseDTO(string message){
        Message = message;
    }
}