using ChatFlow.Application.Services;

namespace ChatFlow.Middleware
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly WebSocketHandler _handler;
        public WebSocketMiddleware(RequestDelegate next, WebSocketHandler handler)
        {
            _next = next;
            _handler = handler;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == "/ws")
            {
                try
                {
                    await _handler.HandleWebSocketAsync(context);
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync(ex.Message);
                    return;
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}
