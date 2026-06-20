using KASHOP.DAL.DTO.Response;

namespace KASHOP.PL.Midlleware
{
    public class GlobalEceptionHandling
    {
        private readonly RequestDelegate _next;

        public GlobalEceptionHandling(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var errorDetails = new ErrorDetails
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = ex.Message,
                    InnerError = ex.InnerException.Message
                };
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(errorDetails);
            }
        }
    }
}
