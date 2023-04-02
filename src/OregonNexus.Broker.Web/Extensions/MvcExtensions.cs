using Microsoft.AspNetCore.Mvc.Rendering;

namespace OregonNexus.Broker.Web;

public static class MvcExtensions
{
    public static string ActiveClass(this IHtmlHelper htmlHelper, string? controllers, string? actions, string cssActiveClass = "active", string cssRegularClass = "link")
    {
        var currentController = htmlHelper?.ViewContext.RouteData.Values["controller"] as string;

        var acceptedControllers = (controllers ?? currentController ?? "").Split(',');

        return acceptedControllers.Contains(currentController)
            ? cssActiveClass
            : cssRegularClass;
    }

    public static string ActiveClassForAction(this IHtmlHelper htmlHelper, string? controllers, string? actions, string cssActiveClass = "active", string cssRegularClass = "link")
    {
        var currentController = htmlHelper?.ViewContext.RouteData.Values["controller"] as string;
        var currentAction = htmlHelper?.ViewContext.RouteData.Values["action"] as string;

        var acceptedControllers = (controllers ?? currentController ?? "").Split(',');
        var acceptedActions = (actions ?? currentAction ?? "").Split(',');

        return acceptedControllers.Contains(currentController) && acceptedActions.Contains(currentAction)
            ? cssActiveClass
            : cssRegularClass;
    }
}