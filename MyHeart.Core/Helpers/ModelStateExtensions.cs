using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyHeart.Core.Helpers
{
    public static class ModelStateExtensions
    {
        public static List<string> GetErrorMessages(this ModelStateDictionary dictionary)
        {
            return dictionary.SelectMany(m => m.Value.Errors)
                             .Select(m => m.ErrorMessage)
                             .ToList();
        }

        public static void AddModelErrors(this ModelStateDictionary modelState, IEnumerable<string> validationResults)
        {
            if (validationResults == null)
                return;

            foreach (var validationResult in validationResults)
            {
                modelState.AddModelError(string.Empty, validationResult);
            }
        }
    }
}
