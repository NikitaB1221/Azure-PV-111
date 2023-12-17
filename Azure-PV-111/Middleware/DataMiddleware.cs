using System.Collections;

namespace Azure_PV_111.Middleware
{
    public class DataMiddleware : IMiddleware
    {
        public static int LifeTime { get; private set; } = 10;
        private static readonly List<LimitedData> Data = new();
        public static void Add(String data)
        {
            Data.Add(new()
            {
                Data = data,
                Moment = DateTime.Now,
            });
        }
        public static List<String> GetData()
        {
            return Data
            .Where(x => x.Moment.AddSeconds(LifeTime) > DateTime.Now)
            .Select(x => x.Data)
            .ToList();
        }

        public static void RemoveExpired()
        {
            Console.WriteLine("RemoveExpired");
        }

        /*
        private readonly RequestDelegate _next;



        public DataMiddleware(RequestDelegate next)
        {
            _next = next;
            Data = new();
        }



        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);
        }
        */
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            return next.Invoke(context);
        }
    }



    public class LimitedData
    {
        public String Data { get; set; } = null!;
        public DateTime Moment { get; set; }
    }
}