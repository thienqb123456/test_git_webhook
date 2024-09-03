namespace ngaoda
{
    public class CustomResponseHeaderMiddleware
    {

        private readonly RequestDelegate _next;

        public CustomResponseHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.OnStarting(() =>
            {
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                return Task.CompletedTask;
            });

            await _next(context);
        }

    }
}
