using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.Error;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate Next;
        private readonly ILogger<ExceptionMiddleware> Logger;
        private readonly IHostEnvironment Env;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            Next = next;
            Logger = logger;
            Env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            try
            {
                await Next(context);
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                context.Response.ContentType ="application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = Env.IsDevelopment()
                ? new ApiException(context.Response.StatusCode,ex.Message,ex.StackTrace?.ToString())
                : new ApiException(context.Response.StatusCode, ex.Message,"Internal Server Error");


                var options = new JsonSerializerOptions{PropertyNamingPolicy= JsonNamingPolicy.CamelCase};

                var json = JsonSerializer.Serialize(response,options);

                await context.Response.WriteAsync(json);

            }
        }




    }
}