using TodoApi.DTOs.TodoDTO;
using TodoApi.Models;
using Riok.Mapperly.Abstractions;
namespace TodoApi.Mapper;
[Mapper(AllowNullPropertyAssignment = false, ThrowOnPropertyMappingNullMismatch = false)]
public partial class TodoMapper(){
    public partial TodoResponseDTO TodoModelToTodoResponseDTO(TodoItem todo);
    public partial TodoItem TodoRequestDtoToModel(TodoRequestDTO request);

    public partial void TodoUpdateRequestDTOToModel(TodoUpdateRequestDTO request, TodoItem entity);
}