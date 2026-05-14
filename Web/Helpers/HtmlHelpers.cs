using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.Helpers
{
    public record BreadcrumbItem(string Label, string? Url = null);

    public static class HtmlHelpers
    {
        public static string IsActive(this IHtmlHelper htmlHelper, string controller, string action)
        {
            var routeData = htmlHelper.ViewContext.RouteData;
            var routeAction = routeData.Values["action"]?.ToString();
            var routeController = routeData.Values["controller"]?.ToString();

            var isCurrent = string.Equals(controller, routeController, System.StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(action, routeAction, System.StringComparison.OrdinalIgnoreCase);

            return isCurrent ? "active" : "";
        }
    }
}
 