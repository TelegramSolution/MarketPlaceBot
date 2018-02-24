using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace MyTelegramBot.Model
{
    public class CustomExceptionFilterAttribute : Attribute, IExceptionFilter
    {

        public void OnException(ExceptionContext context)
        {
            Controllers.HomeController home = new Controllers.HomeController();
            string actionName = context.ActionDescriptor.DisplayName;
            string exceptionStack = context.Exception.StackTrace;
            string exceptionMessage = context.Exception.Message;
            context.Result = home.Error(exceptionMessage);
            context.ExceptionHandled = true;
        }
    }

    
}
