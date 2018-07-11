using GrandBazaar.WebClient.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GrandBazaar.WebClient.Filters
{
    public class AccountAddressFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.Controller as ItemsControllerBase;
            if (controller != null)
            {
                bool addressFound = context.HttpContext.Request.Cookies
                    .TryGetValue("web3-account", out string address);
                if (addressFound)
                {
                    controller.SetAccountAddress(address);
                }
            }
        }
    }
}
