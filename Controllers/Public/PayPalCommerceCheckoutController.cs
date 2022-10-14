using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Http.Extensions;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Web.Controllers;
using Nop.Web.Factories;

using Newtonsoft.Json;

using Nop.Plugin.Payments.PayPalCommerceSSI.Models;
using Nop.Plugin.Payments.PayPalCommerceSSI.Settings;

namespace Nop.Plugin.Payments.PayPalCommerceSSI.Controllers.Public
{
    public partial class PayPalCommerceSSICheckoutController : CheckoutController
    {

        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressService _addressService;
        private readonly ICheckoutModelFactory _checkoutModelFactory;
        private readonly ICountryService _countryService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPaymentService _paymentService;
        private readonly IProductService _productService;
        private readonly IShippingService _shippingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;
        private readonly PaymentSettings _paymentSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly IEncryptionService _encryptionService;
        private readonly PayPalCommerceSettingsSSI _payPalCommerceSettings;

        #endregion

        #region Ctor

        public PayPalCommerceSSICheckoutController(AddressSettings addressSettings,
            CustomerSettings customerSettings,
            IAddressAttributeParser addressAttributeParser,
            IAddressService addressService,
            ICheckoutModelFactory checkoutModelFactory,
            ICountryService countryService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            ILogger logger,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IPaymentPluginManager paymentPluginManager,
            IPaymentService paymentService,
            IProductService productService,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IWebHelper webHelper,
            IWorkContext workContext,
            OrderSettings orderSettings,
            PaymentSettings paymentSettings,
            RewardPointsSettings rewardPointsSettings,
            ShippingSettings shippingSettings,
            IEncryptionService encryptionService,
            PayPalCommerceSettingsSSI payPalCommerceSettings) : base (addressSettings,
                customerSettings,
                addressAttributeParser,
                addressService,
                checkoutModelFactory,
                countryService,
                customerService,
                genericAttributeService,
                localizationService,
                logger,
                orderProcessingService,
                orderService,
                paymentPluginManager,
                paymentService,
                productService,
                shippingService,
                shoppingCartService,
                storeContext,
                webHelper,
                workContext,
                orderSettings,
                paymentSettings,
                rewardPointsSettings,
                shippingSettings)
        {
            _addressSettings = addressSettings;
            _customerSettings = customerSettings;
            _addressAttributeParser = addressAttributeParser;
            _addressService = addressService;
            _checkoutModelFactory = checkoutModelFactory;
            _countryService = countryService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _logger = logger;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _paymentPluginManager = paymentPluginManager;
            _paymentService = paymentService;
            _productService = productService;
            _shippingService = shippingService;
            _shoppingCartService = shoppingCartService;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _webHelper = webHelper;
            _workContext = workContext;
            _orderSettings = orderSettings;
            _paymentSettings = paymentSettings;
            _rewardPointsSettings = rewardPointsSettings;
            _shippingSettings = shippingSettings;
            _encryptionService = encryptionService;
            _payPalCommerceSettings = payPalCommerceSettings;
        }

        #endregion

        #region Utilities

        #endregion

        #region Methods

        public async virtual Task<IActionResult> PayPalCommerceRedirect(string? response)
        {
            string message = "";
            string status = string.Empty;
            string redirectUrl = _webHelper.GetStoreLocation();
            var processPaymentResult = new ProcessPaymentResult();

            var processPaymentRequest = HttpContext.Session.Get<ProcessPaymentRequest>("OrderPaymentInfo");
            //var responseValue = _webHelper.QueryString<string>("response");

            if (processPaymentRequest != null)
            {
                try
                {
                    if (_payPalCommerceSettings.TestMode)
                    {
                        message = "PayPalCommerce Payment Successful";
                        string fullMessage = "Response: " + response;
                        await _logger.InsertLogAsync(LogLevel.Debug, message, fullMessage);
                    }

                    return await ConfirmPayPalCommerceOrder(response);

                }
                catch (Exception exc)
                {
                    message = "PayPalCommerce Payment Exception";
                    string fullMessage = "Response: " + response;
                    await _logger.InsertLogAsync(LogLevel.Error, message, fullMessage);
                }
            }

            var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(),
                ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStoreAsync()).Id);

            if (cart.Any())
            {
                if (_orderSettings.OnePageCheckoutEnabled)
                    return RedirectToRoute("CheckoutOnePage");
                else
                    return RedirectToRoute("ShoppingCart");
            }

            return RedirectToRoute("HomePage");
        }

        public virtual async Task<IActionResult> ConfirmPayPalCommerceOrder(string transactionId)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            //model
            var model = await _checkoutModelFactory.PrepareConfirmOrderModelAsync(cart);
            try
            {
                //prevent 2 orders being placed within an X seconds time frame
                if (!await IsMinimumOrderPlacementIntervalValidAsync(customer))
                    throw new Exception(await _localizationService.GetResourceAsync("Checkout.MinOrderPlacementInterval"));

                //place order
                var processPaymentRequest = HttpContext.Session.Get<ProcessPaymentRequest>("OrderPaymentInfo");
                if (processPaymentRequest != null && processPaymentRequest.CustomValues.Count == 0)
                {
                    //save order details for future using
                    var key = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalCommerceSSI.OrderId");
                    processPaymentRequest.CustomValues.Add(key, transactionId);
                }


                processPaymentRequest.StoreId = store.Id;
                processPaymentRequest.CustomerId = customer.Id;
                processPaymentRequest.PaymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(customer,
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);

                HttpContext.Session.Set<ProcessPaymentRequest>("OrderPaymentInfo", processPaymentRequest);
                var placeOrderResult = await _orderProcessingService.PlaceOrderAsync(processPaymentRequest);
                if (placeOrderResult.Success)
                {
                    HttpContext.Session.Set<ProcessPaymentRequest>("OrderPaymentInfo", null);
                    var postProcessPaymentRequest = new PostProcessPaymentRequest
                    {
                        Order = placeOrderResult.PlacedOrder
                    };

                    postProcessPaymentRequest.Order.AuthorizationTransactionId = transactionId;

                    await _paymentService.PostProcessPaymentAsync(postProcessPaymentRequest);

                    return RedirectToRoute("CheckoutCompleted", new { orderId = placeOrderResult.PlacedOrder.Id });
                }

                foreach (var error in placeOrderResult.Errors)
                    model.Warnings.Add(error);
            }
            catch (Exception exc)
            {
                await _logger.WarningAsync(exc.Message, exc);
                model.Warnings.Add(exc.Message);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

    }
}