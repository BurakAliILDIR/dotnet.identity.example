using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IdentityExample.Web.Extensions
{
    public static class ModelStateExtensions
    {
        public static void AddModelErrors(this ModelStateDictionary modelStateDictionary, List<string> errors)
        {
            foreach (string error in errors)
            {
                modelStateDictionary.AddModelError(string.Empty, error);
            }
        }
    }
}