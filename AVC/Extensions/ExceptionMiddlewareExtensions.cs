using AVC.Dtos.ReponseDtos;
using AVC.Extensions;
using AVC.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;

namespace AVC.Extension
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILogger logger, IWebHostEnvironment env)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerPathFeature>();
                    if (exception?.Error is NotFoundException)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    }
                    else if (exception?.Error is PermissionDeniedException)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    }
                    else if (exception?.Error is UnauthorizedAccessException )
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    }
                    else if (exception?.Error is ConflictEntityException)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        logger.LogError($"Something went wrong: {exception.Error}");
                    }
                    context.Response.ContentType = "application/json";

                    if (exception != null)
                    {
                        //var errorDto = new ResponseDto(context.Response.StatusCode == (int)HttpStatusCode.InternalServerError ? (env.IsDevelopment() ? exception?.Error.Message : "Interval server error") : exception?.Error.Message);
                        var errorDto = new ResponseDto(exception?.Error.StackTrace);

                        //await context.Response.WriteAsync(new ErrorDetails()
                        //{
                        //    StatusCode = context.Response.StatusCode,
                        //    Message = "Interval Server Error"
                        //}.ToString()); ;
                        await context.Response.WriteAsync(errorDto.ToString());
                    }
                });
            });
        }
    }
}
