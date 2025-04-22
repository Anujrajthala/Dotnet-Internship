using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Core.Events;
using TodoApi.Exceptions;
public class GlobalExceptionHandlerMiddleware{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly Dictionary<Type, Func<Exception,HttpContext,Task>> _exceptionHandlers;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger){
        _next = next;
        _logger = logger;
        _exceptionHandlers = new Dictionary<Type,Func<Exception,HttpContext,Task>>(){
            {typeof(BadRequestException),(ex,context)=> HandleBadRequestException((BadRequestException)ex,context)},
            // {typeof(ForbiddenException),(ex,context)=> HandleForbiddenException((ForbiddenException) ex,context)},
            // {typeof(UnAuthorizedException),(ex,context)=> HandleUnAuthorizedException((UnAuthorizedException)ex,context)},
            {typeof(NotFoundException), (ex,context)=> HandleNotFoundException((NotFoundException)ex,context)},
            
        };
    }
    public async Task Invoke(HttpContext context){
        try{
            await _next(context);
        }
        catch(Exception ex){
            
            await HandleExceptionAsync(context,ex);
            return;
        }

    }
    public async Task HandleExceptionAsync(HttpContext context, Exception ex){
        if(_exceptionHandlers.TryGetValue(ex.GetType(),out var handler)){
            await handler (ex,context);
        }
        else{
            await HandleGenericException(ex,context);

        }
    }

    public Task HandleBadRequestException(BadRequestException ex,HttpContext context){
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        var errorResponse = new{
            success = false,
            statusCode = 400,
            message = ex.Message,
            Error = ex.Errors
        };

        return context.Response.WriteAsJsonAsync(errorResponse);
    }
    public Task HandleNotFoundException(NotFoundException ex, HttpContext context){
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        var errorResponse = new{
            success = false,
            statusCode= 404,
            message = ex.Message
        };
        return context.Response.WriteAsJsonAsync(errorResponse);
    }
    public Task HandleGenericException(Exception ex, HttpContext context){
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        var errorResponse = new{
            success = false,
            statusCode = 500,
            message = "An unexpected error occurred."
        };
        return context.Response.WriteAsJsonAsync(errorResponse);
    }

}