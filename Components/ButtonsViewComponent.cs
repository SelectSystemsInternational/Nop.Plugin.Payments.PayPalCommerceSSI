using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Services.Payments;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Models.Catalog;

using Nop.Plugin.Payments.PayPalCommerceSSI.Services;
using Nop.Plugin.Payments.PayPalCommerceSSI.Settings;

namespace Nop.Plugin.Payments.PayPalCommerceSSI.Components
{
    /// <summary>
    /// Represents the view component to display buttons
    /// </summary>
    [ViewComponent(Name = PayPalCommerceDefaults.BUTTONS_VIEW_COMPONENT_NAME)]
    public class ButtonsViewComponent : NopViewComponent
    {
        #region Fields

        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly PayPalCommerceSettingsSSI _settings;

        #endregion

        #region Ctor

        public ButtonsViewComponent(IPaymentPluginManager paymentPluginManager,
            IStoreContext storeContext,
            IWorkContext workContext,
            PayPalCommerceSettingsSSI settings)
        {
            _paymentPluginManager = paymentPluginManager;
            _storeContext = storeContext;
            _workContext = workContext;
            _settings = settings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the view component result
        /// </returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            if (!await _paymentPluginManager.IsPluginActiveAsync(PayPalCommerceDefaults.SystemName, customer, store?.Id ?? 0))
                return Content(string.Empty);

            if (!ServiceManager.IsConfigured(_settings))
                return Content(string.Empty);

            if (!widgetZone.Equals(PublicWidgetZones.ProductDetailsAddInfo) && !widgetZone.Equals(PublicWidgetZones.OrderSummaryTotals))
                return Content(string.Empty);

            if (widgetZone.Equals(PublicWidgetZones.OrderSummaryTotals))
            {
                if (!_settings.DisplayButtonsOnShoppingCart)
                    return Content(string.Empty);

                var routeName = HttpContext.GetEndpoint()?.Metadata.GetMetadata<RouteNameMetadata>()?.RouteName;
                if (!routeName.Contains(PayPalCommerceDefaults.ShoppingCartRouteName))
                    return Content(string.Empty);
            }

            if (widgetZone.Equals(PublicWidgetZones.ProductDetailsAddInfo) && !_settings.DisplayButtonsOnProductDetails)
                return Content(string.Empty);

            var productId = additionalData is ProductDetailsModel.AddToCartModel model ? model.ProductId : 0;
            return View("~/Plugins/SSI.Payments.PayPalCommerce/Views/Buttons.cshtml", (widgetZone, productId));
        }

        #endregion
    }
}