using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ForumSiteCore.BlazorWasm.Server.Utility
{
    public static class ModelStateHelper
    {
        public static Dictionary<string, string[]> Errors(this ModelStateDictionary modelState)
        {
            if (!modelState.IsValid)
            {
                var errorList = modelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

                return errorList;
            }
            return null;
        }
    }
}
