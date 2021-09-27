using LogExceptions_CustomMiddleware.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace LogExceptions_CustomMiddleware.ExcceptionHandler
{
    public static class ExceptionMiddlewareHandler
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILogger logger, AppDbContext db)
        
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        logger.LogError($"Something went wrong: {contextFeature.Error.Message},{contextFeature.Error.Source},{contextFeature.Error.TargetSite},{contextFeature.Error.Data},{contextFeature.Error.HResult},{contextFeature.Error.GetType()}");
                        var trace = new StackTrace(contextFeature.Error, true);
                        var frame = trace.GetFrames().FirstOrDefault();
                        var lineNumber = frame.GetFileLineNumber();
                        var fileName = frame.GetFileName();
                        Logs logs = new();
                        logs.Source = contextFeature.Error.Source;
                        logs.FilePath = fileName;
                        logs.LineNumber = Convert.ToInt32(lineNumber);
                        logs.ExceptionMessage = contextFeature.Error.Message;
                        logs.CretedDate = DateTime.UtcNow;
                        db.Logs.Add(logs);
                        db.SaveChanges();
                        // await SaveLogsToDatabase($"File Name : {fileName}, Line number : {lineNumber}, Exception : {contextFeature.Error.Message}", contextFeature.Error.Source);
                        await context.Response.WriteAsync(new ErrorDetails
                        {
                            StatusCode = context.Response.StatusCode,
                            Exception = $"File Name : {fileName}, Line number : {lineNumber}, Exception : {contextFeature.Error.Message}"
                        }.ToString());
                    }
                });
            });
        }
    }

    
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Exception { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
