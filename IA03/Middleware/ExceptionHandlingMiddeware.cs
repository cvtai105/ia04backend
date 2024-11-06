using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace IA03.Middleware
{
    public class ExceptionHandlingMiddeware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddeware> _logger;
        public ExceptionHandlingMiddeware(RequestDelegate next, ILogger<ExceptionHandlingMiddeware> logger)
        {
            _next = next;
            _logger = logger;
        }
    
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UnauthorizedAccessException){
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Title = "Unauthorized",
                    Status = StatusCodes.Status401Unauthorized,
                    Detail = "You are not authorized to access this resource"
                });
            }
            catch (ArgumentException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                _logger.LogWarning(ex, "Bad request: {Message}", ex.Message);
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Title = "Bad Request",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unknown exception occurred: {Message}",ex.Message);
                var problemDetails = new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = ex.StackTrace
                };

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        }

    }
}