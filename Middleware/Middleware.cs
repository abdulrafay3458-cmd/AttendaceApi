using AttendanceAPI.Interface;
using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace AttendanceAPI.Middleware
{
    public class Middleware
    {
        private readonly RequestDelegate _next;

        public Middleware(RequestDelegate next)
        {
            _next = next;
        }
         
        public async Task Invoke(HttpContext context)
        {
            try
            {
                var endpoint = context.GetEndpoint();
                if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() is object)
                {
                    await _next(context);
                    return;
                }
                else
                {
                    if (context.Request.Method != HttpMethods.Get)
                    {
                        var empCode = context.User.Claims.First(c => c.Type == "employeeId").Value;
                        var deviceHash = context.User.Claims.FirstOrDefault(c => c.Type == "deviceHash");
                        var hash = context.Request.Headers["device_Hash"].ToString();
                        if (string.IsNullOrWhiteSpace(hash) || deviceHash == null || deviceHash.Value != hash)
                        {
                            await RejectRequest(context);
                            return;
                        }

                        //if (deviceHash.Value != hash)
                        //{
                        //    await RejectRequest(context);
                        //    return;
                        //}
                    }
                    await _next(context);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Unhandled Exception");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync(e.Message);
                return;
            }
        }

        private static async Task RejectRequest(HttpContext context)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Unauthorized Device");
        }
    }
}
