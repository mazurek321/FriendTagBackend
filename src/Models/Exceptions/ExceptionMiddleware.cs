namespace FriendTagBackend.src.Exceptions;
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (CustomException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var response = new { message = ex.Message };
            var json = System.Text.Json.JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new
            {
                message = _env.IsDevelopment() ? ex.Message : "An unexpected error occurred."
            };
            var json = System.Text.Json.JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
    }
}
