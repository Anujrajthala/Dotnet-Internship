using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApi.DTOs.TodoDTO;
using TodoApi.Services;
[Route("api/todo")]
[Authorize]
[ApiController]
public class TodoController: ControllerBase{
    private readonly ITodoService _todoService;

    public TodoController(ITodoService todoService){

         _todoService = todoService;
    }

    [HttpPost]
    public async Task<IActionResult> AddTodo([FromBody] TodoRequestDTO request){
        var response = await _todoService.AddTodoItem(request);
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetTodoItemById([FromQuery] string id){
        var response = await _todoService.GetTodoById(id);
        return Ok(response);
    }
    [HttpGet("getall")]
    public async Task<IActionResult> GetAllTodo(){
        TodoListResponseDTO response  = await _todoService.GetTodoList();
        Console.WriteLine($"response is : {response.Response}");
        return Ok(response);
    }
    [HttpPut]
    public async Task<IActionResult> UpdateTodoItem([FromBody] TodoUpdateRequestDTO request,[FromQuery] string id){
        var response = await _todoService.UpdateTodoItem(request,id);
        return Ok(response);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteTodoItem([FromQuery] string id){
        var response  = await _todoService.DeleteTodoItem(id);
        return Ok(response);
    }
    [HttpPatch]
    public async Task<IActionResult> CompleteTodo([FromQuery] string id){
        var response = await _todoService.CompleteTodo(id);
        return Ok(response);
    }
}