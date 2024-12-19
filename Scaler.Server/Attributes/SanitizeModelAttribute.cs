﻿using Microsoft.AspNetCore.Mvc.Filters;

namespace Scaler.Server.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SanitizeModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            foreach (var arg in context.ActionArguments.Values)
            {
                if (arg is ISanitizeModel model)
                {
                    model.SanitizeModel();
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface ISanitizeModel
    {
        public void SanitizeModel();
    }
}
