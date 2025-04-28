namespace TodoApi.DTOs.TodoDTO;
public class TodoResponseDTO{
    public string Id{get; set;}
    public string Title{get; set;} = string.Empty;
    public string Description{get; set;}= string.Empty;
    public DateTime AddedAt{get; set;}
    public DateTime ExpiryDate{get; set;}
}