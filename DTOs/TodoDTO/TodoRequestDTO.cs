public class TodoRequestDTO{
    public string Title{get; set;} = string.Empty;
    public string Description{get; set;} = string.Empty;
    public DateTime ExpiryDate{get; set;}
}