using MongoDB.Driver;
using TodoApi.Data;
using TodoApi.Models;
using TodoApi.DTOs.TodoDTO;
using TodoApi.Exceptions;
using System.Security.Claims;
using TodoApi.Mapper;
using TodoApi.DTOs;
using ZstdSharp;

namespace TodoApi.Services.ServiceImpl;
public class TodoService: ITodoService{
    private readonly IMongoCollection<TodoItem> _todoitems;
    private readonly IHttpContextAccessor _accessor;
    private readonly ILogger<TodoService> _logger;
    private readonly TodoMapper _mapper;
    public TodoService(ApplicationDbContext applicationDbContext,IHttpContextAccessor accessor, TodoMapper mapper, ILogger<TodoService> logger){
        _todoitems = applicationDbContext.TodoItems;
        _accessor = accessor;
        _mapper = mapper;
        _logger = logger;
    }

      public string? GetCurrentUserId(){
        var userId = _accessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
         Console.WriteLine($"the userid is {userId}");
         return userId;
    }

    public async Task<TodoResponseDTO> AddTodoItem(TodoRequestDTO request){
        var userId = GetCurrentUserId(); 
        
        var todoitem = await _todoitems.Find(t=>t.Title==request.Title && t.UserId.Equals(userId) && t.ExpiryDate>DateTime.UtcNow && !t.IsCompleted).AnyAsync();
        if(todoitem){
            throw new BadRequestException($"Todo Item with Title '{request.Title}' already exists and still pending.");
        }  
        var newTodo = _mapper.TodoRequestDtoToModel(request);
        newTodo.AddedAt = DateTime.UtcNow;
        newTodo.IsCompleted = false;
        newTodo.UserId = userId;
        await _todoitems.InsertOneAsync(newTodo);
        return _mapper.TodoModelToTodoResponseDTO(newTodo);;
    }

    public async Task<TodoResponseDTO> GetTodoById(string id){
        var userId = GetCurrentUserId();
        var todo = await _todoitems.Find(t=>t.Id.Equals(id)).FirstOrDefaultAsync();
        if(todo.UserId!=userId){
            throw new UnAuthorizedException("You are not allowed to view the todo of another user.");
        }
        return _mapper.TodoModelToTodoResponseDTO(todo);
    }
    public async Task<TodoListResponseDTO> GetTodoList(){
         
        List<TodoResponseDTO> response = [];
        var userId = GetCurrentUserId();
        List<TodoItem> todoItems = await _todoitems.Find(t=>t.UserId.Equals(userId)).ToListAsync();
        Console.WriteLine(todoItems);
        if(todoItems.Count()== 0){
            return new TodoListResponseDTO("There is no todos for this user");
        }
        todoItems.ForEach(todo=> response.Add(_mapper.TodoModelToTodoResponseDTO(todo)));

        return new TodoListResponseDTO(response,"Todos successfully retreived");
    }
    public async Task<TodoUpdateResponseDTO> UpdateTodoItem(TodoUpdateRequestDTO request, string id){
        var userId = GetCurrentUserId();
        var todoitemExists = await _todoitems.Find(t=>t.Title==request.Title && t.Id!=id && t.UserId.Equals(userId) && t.ExpiryDate>DateTime.UtcNow && !t.IsCompleted).AnyAsync();
        if(todoitemExists) return new TodoUpdateResponseDTO(false,$"Todo with title {request.Title} already exists and is pending.");
        var existingTodo = await _todoitems.Find(t=> t.Id.Equals(id)).FirstOrDefaultAsync() ?? throw new NotFoundException($"Todo with id {id} does not exist");
        if (existingTodo.UserId!=userId){
            throw new UnAuthorizedException("User is not allowed to make changes to another user's todo items.");
        }
        _mapper.TodoUpdateRequestDTOToModel(request,existingTodo);
        await _todoitems.ReplaceOneAsync(t=>t.Id==existingTodo.Id,existingTodo);
        return new TodoUpdateResponseDTO(true,$"Todo with id '{id}' updated successfully.");

    }
    public async Task<TodoDeleteResponseDTO> DeleteTodoItem(string id){
        var userId = GetCurrentUserId();
        var existingTodo = await _todoitems.Find(t=> t.Id == id).FirstOrDefaultAsync()??throw new NotFoundException($"Todo with id '{id} could not be found.'");
        if(existingTodo.UserId!= userId) throw new UnAuthorizedException("User is not allowed to delete another user's todo items");
        await _todoitems.DeleteOneAsync(t=> t.Id==id);
        return new TodoDeleteResponseDTO(true, $"Todo with id '{id}' deleted successfully.");
    }

    public async Task<TodoUpdateResponseDTO> CompleteTodo(string id){
        var userId = GetCurrentUserId();
        var existingTodo = await _todoitems.Find(t=>t.Id==id&&t.UserId==userId).FirstOrDefaultAsync()??throw new NotFoundException($"Todo with id '{id}' not found");
        if(existingTodo.IsCompleted){
            return new TodoUpdateResponseDTO(false,"Todo was already completed already.");
        }
        var filter = Builders<TodoItem>.Filter.Eq(t=>t.Id,id);
        var update = Builders<TodoItem>.Update.Set(t=>t.IsCompleted,true);
        await _todoitems.UpdateOneAsync(filter,update);
        return new TodoUpdateResponseDTO(true,$"Todo was completed successfully for todo with Title: {existingTodo.Title}");
    }
}