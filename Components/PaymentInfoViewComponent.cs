using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Http.Extensions;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Payments;
using Nop.Web.Framework.Components;

using Nop.Plugin.Payments.PayPalCommerceSSI.Models;
using Nop.Plugin.Payments.PayPalCommerceSSI.Services;
using Nop.Plugin.Payments.PayPalCommerceSSI.Settings;

namespace Nop.Plugin.Payments.PayPalCommerceSSI.Components
{
    /// <summary>
    /// Represents the view component to display payment info in public store
    /// </summary>
    [ViewComponent(Name = PayPalCommerceDefaults.PAYMENT_INFO_VIEW_COMPONENT_NAME)]
    public class PaymentInfoViewComponent : NopViewComponent
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPaymentService _paymentService;
        private readonly OrderSettings _orderSettings;
        private readonly PayPalCommerceSettingsSSI _settings;
        private readonly ServiceManager _serviceManager;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public PaymentInfoViewComponent(IWorkContext workContext,
            IStoreContext storeContext, 
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPaymentService paymentService,
            OrderSettings orderSettings,
            PayPalCommerceSettingsSSI settings,
            ServiceManager serviceManager,
            IGenericAttributeService genericAttributeService,
            ILogger logger)
        {
            _workContext = workContext;
            _storeContext = storeContext;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _paymentService = paymentService;
            _orderSettings = orderSettings;
            _settings = settings;
            _serviceManager = serviceManager;
            _genericAttributeService = genericAttributeService;
            _logger = logger;
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
            var paymentMethodSystemName = _genericAttributeService.GetAttributeAsync<string>(_workContext.GetCurrentCustomerAsync().Result,
                NopCustomerDefaults.SelectedPaymentMethodAttribute, (_storeContext.GetCurrentStoreAsync().Result).Id).Result;
            if (paymentMethodSystemName != null && paymentMethodSystemName.Equals(PayPalCommerceDefaults.SystemName))
            {

                var model = new PaymentInfoModel();

                //prepare order GUID
                var paymentRequest = new ProcessPaymentRequest();
                _paymentService.GenerateOrderGuid(paymentRequest);

                //try to create an order
                var (order, error) = await _serviceManager.CreateOrderAsync(_settings, paymentRequest.OrderGuid);
                if (order != null)
                {
                    model.OrderId = order.Id;

                    if (_settings.TestMode)
                    {
                        string message = "PayPalCommerce Create Order Successful";
                        string fullMessage = "Order Guid: " + paymentRequest.OrderGuid + " Order Id: " + order.Id;
                        await _logger.InsertLogAsync(LogLevel.Debug, message, fullMessage);
                    }

                    //save order details for future using
                    var key = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerceSSI.OrderId");
                    paymentRequest.CustomValues.Add(key, order.Id);
                }
                else if (!string.IsNullOrEmpty(error))
                {
                    model.Errors = error;
                    if (_orderSettings.OnePageCheckoutEnabled)
                        ModelState.AddModelError(string.Empty, error);
                    else
                        _notificationService.ErrorNotification(error);
                }

                HttpContext.Session.Set(PayPalCommerceDefaults.PaymentRequestSessionKey, paymentRequest);
                return View("~/Plugins/SSI.Payments.PayPalCommerce/Views/PaymentInfo.cshtml", model);
            }

            //dont display widget
            return Content(string.Empty);
        }

        #endregion

    }
}