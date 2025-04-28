namespace TodoApi.DTOs.TodoDTO;
public class TodoUpdateRequestDTO{
    public string? Title{get; set;}
    public string? Description{get; set;}
    public DateTime? ExpiryDate{get; set;}
}