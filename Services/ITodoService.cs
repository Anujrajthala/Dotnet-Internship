using TodoApi.DTOs.TodoDTO;

namespace TodoApi.Services;
public interface ITodoService{
    public Task<TodoResponseDTO> AddTodoItem(TodoRequestDTO requestDTO);
    public Task<TodoResponseDTO> GetTodoById(string id);
    public Task<TodoListResponseDTO> GetTodoList();
    public Task<TodoUpdateResponseDTO> UpdateTodoItem(TodoUpdateRequestDTO request, string id);
    public Task<TodoDeleteResponseDTO> DeleteTodoItem(string id);
    public Task<TodoUpdateResponseDTO> CompleteTodo(string id);
}