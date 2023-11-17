namespace Azure_PV_111.Middleware
{
    public class DataMiddleware : IMiddleware
    {
        public static List<String> Data { get; set; } = null!;

        //private readonly RequestDelegate _next; 
        public DataMiddleware(/*RequestDelegate next*/)
        {
            //_next = next;
            Data = new List<String>();
        }
        //public async Task InvokeAsync(HttpContext context)
        //{
        //    await _next(context);
        //}

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            return next.Invoke(context);
        }
    }
}
