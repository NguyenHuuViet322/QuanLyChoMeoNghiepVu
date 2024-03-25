using ESD.Domain.Interfaces.DAS;
using ESD.Domain.Models.CustomModels;
using ESD.Infrastructure.Constants;
using ESD.Utility.LogUtils;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ESD.Application.Middwares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerManager _logger;
        private readonly ILogBySqlRepository _logBySql;

        //public ExceptionMiddleware(RequestDelegate next, ILoggerManager logger, ILogBySqlRepository logBySql)
        //{
        //    _logger = logger;
        //    _next = next;
        //    _logBySql = logBySql;
        //}
        public ExceptionMiddleware(RequestDelegate next, ILoggerManager logger)
        {
            _logger = logger;
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                await Task.Run(() => { HandleExceptionAsync(httpContext, ex); });
            }
            if (httpContext.Response.StatusCode == StatusCodes.Status404NotFound)
            {
                httpContext.Request.Path = "/Error/Error404";
                await _next(httpContext);
            }
        }

        private void  HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var id =  System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(exception.Message));
            context.Response.Redirect($"/Error/Error500?id={id}");
        }
    }
}
