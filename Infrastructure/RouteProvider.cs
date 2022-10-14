using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure;

namespace Nop.Plugin.Payments.PayPalCommerceSSI.Infrastructure
{
    /// <summary>
    /// Represents plugin route provider
    /// </summary>
    public class RouteProvider : BaseRouteProvider, IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            var lang = GetLanguageRoutePattern();

            endpointRouteBuilder.MapControllerRoute(PayPalCommerceDefaults.ConfigurationRouteName,
                "Admin/PayPalCommerceSSI/Configure",
                new { controller = "PayPalCommerceSSI", action = "Configure" });

            endpointRouteBuilder.MapControllerRoute(PayPalCommerceDefaults.WebhookRouteName,
                "Plugins/PayPalCommerceSSI/Webhook",
                new { controller = "PayPalCommerceSSIWebhook", action = "WebhookHandler" });

            endpointRouteBuilder.MapControllerRoute(PayPalCommerceDefaults.CheckoutRouteName,
                "PayPalCommerceSSI/paymenthandler",
                new { controller = "PayPalCommerceSSICheckout", action = "PayPalCommerceRedirect" });

            endpointRouteBuilder.MapControllerRoute(PayPalCommerceDefaults.CheckoutRouteNamePattern,
                pattern: $"{lang}/PayPalCommerceSSI/paymenthandler",
                defaults: new { controller = "PayPalCommerceSSICheckout", action = "PayPalCommerceRedirect" });

        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;
    }
}