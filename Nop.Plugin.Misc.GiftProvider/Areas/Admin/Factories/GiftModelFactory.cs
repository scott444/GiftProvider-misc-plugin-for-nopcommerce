using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Plugin.Misc.GiftProvider.Areas.Admin.Models.Gifts;
using Nop.Plugin.Misc.GiftProvider.Domain;
using Nop.Plugin.Misc.GiftProvider.Services;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Models.Extensions;
using RequirementGroupInteractionType = Nop.Plugin.Misc.GiftProvider.Domain.RequirementGroupInteractionType;

namespace Nop.Plugin.Misc.GiftProvider.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the discount model factory implementation
    /// </summary>
    public partial class GiftModelFactory : IGiftModelFactory
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICategoryService _categoryService;
        private readonly ICategoryGiftService _categoryGiftService;
        private readonly ICurrencyService _currencyService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IGiftPluginManager _discountPluginManager;
        private readonly IGiftService _discountService;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IManufacturerGiftService _manufacturerGiftService;
        private readonly IOrderService _orderService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductService _productService;
        private readonly IProductGiftService _productGiftService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public GiftModelFactory(CurrencySettings currencySettings,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICategoryService categoryService,
            ICategoryGiftService categoryGiftService,
            ICurrencyService currencyService,
            IDateTimeHelper dateTimeHelper,
            IGiftPluginManager discountPluginManager,
            IGiftService discountService,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            IManufacturerGiftService manufacturerGiftService,
            IOrderService orderService,
            IPriceFormatter priceFormatter,
            IProductService productService,
            IProductGiftService productGiftService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper)
        {
            _currencySettings = currencySettings;
            _baseAdminModelFactory = baseAdminModelFactory;
            _categoryService = categoryService;
            _categoryGiftService = categoryGiftService;
            _currencyService = currencyService;
            _dateTimeHelper = dateTimeHelper;
            _discountPluginManager = discountPluginManager;
            _discountService = discountService;
            _localizationService = localizationService;
            _manufacturerService = manufacturerService;
            _orderService = orderService;
            _priceFormatter = priceFormatter;
            _productService = productService;
            _productGiftService = productGiftService;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare discount usage history search model
        /// </summary>
        /// <param name="searchModel">Gift usage history search model</param>
        /// <param name="discount">Gift</param>
        /// <returns>Gift usage history search model</returns>
        protected virtual GiftUsageHistorySearchModel PrepareGiftUsageHistorySearchModel(GiftUsageHistorySearchModel searchModel,
            Gift discount)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            searchModel.GiftId = discount.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare discount product search model
        /// </summary>
        /// <param name="searchModel">Gift product search model</param>
        /// <param name="discount">Gift</param>
        /// <returns>Gift product search model</returns>
        protected virtual GiftProductSearchModel PrepareGiftProductSearchModel(GiftProductSearchModel searchModel, Gift discount)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            searchModel.GiftId = discount.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare discount category search model
        /// </summary>
        /// <param name="searchModel">Gift category search model</param>
        /// <param name="discount">Gift</param>
        /// <returns>Gift category search model</returns>
        protected virtual GiftCategorySearchModel PrepareGiftCategorySearchModel(GiftCategorySearchModel searchModel, Gift discount)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            searchModel.GiftId = discount.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare discount manufacturer search model
        /// </summary>
        /// <param name="searchModel">Gift manufacturer search model</param>
        /// <param name="discount">Gift</param>
        /// <returns>Gift manufacturer search model</returns>
        protected virtual GiftManufacturerSearchModel PrepareGiftManufacturerSearchModel(GiftManufacturerSearchModel searchModel,
            Gift discount)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            searchModel.GiftId = discount.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare available discount types
        /// </summary>
        /// <param name="items">Discount type items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task PrepareGiftTypesAsync(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available discount types
            var availableGiftTypeItems = await GiftType.AssignedToOrderTotal.ToSelectListAsync(false);
            foreach (var giftTypeItem in availableGiftTypeItems)
            {
                items.Add(giftTypeItem);
            }

            //insert special item for the default value
            await PrepareDefaultItemAsync(items, withSpecialDefaultItem, defaultItemText);
        }
        protected virtual async Task PrepareDefaultItemAsync(IList<SelectListItem> items, bool withSpecialDefaultItem, string defaultItemText = null, string defaultItemValue = "0")
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //whether to insert the first special item for the default value
            if (!withSpecialDefaultItem)
                return;

            //prepare item text
            defaultItemText ??= await _localizationService.GetResourceAsync("Admin.Common.All");

            //insert this default item at first
            items.Insert(0, new SelectListItem { Text = defaultItemText, Value = defaultItemValue });
        }


        /// <summary>
        /// Prepare discount search model
        /// </summary>
        /// <param name="searchModel">Gift search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the discount search model
        /// </returns>
        public virtual async Task<GiftSearchModel> PrepareGiftSearchModelAsync(GiftSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available discount types
            await PrepareGiftTypesAsync(searchModel.AvailableGiftTypes);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged discount list model
        /// </summary>
        /// <param name="searchModel">Gift search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the discount list model
        /// </returns>
        public virtual async Task<GiftListModel> PrepareGiftListModelAsync(GiftSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter discounts
            var discountType = searchModel.SearchGiftTypeId > 0 ? (GiftType?)searchModel.SearchGiftTypeId : null;
            var startDateUtc = searchModel.SearchStartDate.HasValue ?
                (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchStartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()) : null;
            var endDateUtc = searchModel.SearchEndDate.HasValue ?
                (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchEndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1) : null;

            //get discounts
            var discounts = (await _discountService.GetAllGiftsAsync(showHidden: true,
                giftType: discountType,
                couponCode: searchModel.SearchGiftCouponCode,
                giftName: searchModel.SearchGiftName,
                startDateUtc: startDateUtc,
                endDateUtc: endDateUtc)).ToPagedList(searchModel);

            //prepare list model
            var model = await new GiftListModel().PrepareToGridAsync(searchModel, discounts, () =>
            {
                return discounts.SelectAwait(async discount =>
                {
                    //fill in model values from the entity
                    var discountModel = discount.ToModel<GiftModel>();

                    //fill in additional values (not existing in the entity)
                    discountModel.GiftTypeName = await _localizationService.GetLocalizedEnumAsync(discount.GiftType);
                    discountModel.PrimaryStoreCurrencyCode = (await _currencyService
                        .GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId))?.CurrencyCode;
                    discountModel.TimesUsed = (await _discountService.GetAllGiftUsageHistoryAsync(discount.Id, pageSize: 1)).TotalCount;

                    return discountModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare discount model
        /// </summary>
        /// <param name="model">Gift model</param>
        /// <param name="discount">Gift</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the discount model
        /// </returns>
        public virtual async Task<GiftModel> PrepareGiftModelAsync(GiftModel model, Gift discount, bool excludeProperties = false)
        {
            if (discount != null)
            {
                //fill in model values from the entity
                model ??= discount.ToModel<GiftModel>();

                //prepare available discount requirement rules
                var discountRules = await _discountPluginManager.LoadAllPluginsAsync();
                foreach (var discountRule in discountRules)
                {
                    model.AvailableGiftRequirementRules.Add(new SelectListItem
                    {
                        Text = discountRule.PluginDescriptor.FriendlyName,
                        Value = discountRule.PluginDescriptor.SystemName
                    });
                }

                model.AvailableGiftRequirementRules.Insert(0, new SelectListItem
                {
                    Text = await _localizationService.GetResourceAsync("Admin.Promotions.Gifts.Requirements.GiftRequirementType.AddGroup"),
                    Value = "AddGroup"
                });

                model.AvailableGiftRequirementRules.Insert(0, new SelectListItem
                {
                    Text = await _localizationService.GetResourceAsync("Admin.Promotions.Gifts.Requirements.GiftRequirementType.Select"),
                    Value = string.Empty
                });

                //prepare available requirement groups
                var requirementGroups = (await _discountService.GetAllGiftRequirementsAsync(discount.Id)).Where(requirement => requirement.IsGroup);
                model.AvailableRequirementGroups = requirementGroups.Select(requirement =>
                    new SelectListItem { Value = requirement.Id.ToString(), Text = requirement.GiftRequirementRuleSystemName }).ToList();

                //prepare nested search models
                PrepareGiftUsageHistorySearchModel(model.GiftUsageHistorySearchModel, discount);
                PrepareGiftProductSearchModel(model.GiftProductSearchModel, discount);
                PrepareGiftCategorySearchModel(model.GiftCategorySearchModel, discount);
                PrepareGiftManufacturerSearchModel(model.GiftManufacturerSearchModel, discount);
            }

            model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;

            //get URL of discount with coupon code
            if (model.RequiresCouponCode && !string.IsNullOrEmpty(model.CouponCode))
            {
                model.GiftUrl = QueryHelpers.AddQueryString((_webHelper.GetStoreLocation()).TrimEnd('/'),
                    NopGiftDefaults.GiftCouponQueryParameter, model.CouponCode);
            }

            //set default values for the new model
            if (discount == null)
                model.LimitationTimes = 1;

            return model;
        }

        /// <summary>
        /// Prepare discount requirement rule models
        /// </summary>
        /// <param name="requirements">Collection of discount requirements</param>
        /// <param name="discount">Gift</param>
        /// <param name="groupInteractionType">Interaction type within the group of requirements</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of discount requirement rule models
        /// </returns>
        public virtual async Task<IList<GiftRequirementRuleModel>> PrepareGiftRequirementRuleModelsAsync
            (ICollection<GiftRequirement> requirements, Gift discount, RequirementGroupInteractionType groupInteractionType)
        {
            if (requirements == null)
                throw new ArgumentNullException(nameof(requirements));

            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            var lastRequirement = requirements.LastOrDefault();

            return await requirements.SelectAwait(async requirement =>
            {
                //set common properties
                var requirementModel = new GiftRequirementRuleModel
                {
                    GiftRequirementId = requirement.Id,
                    ParentId = requirement.ParentId,
                    IsGroup = requirement.IsGroup,
                    RuleName = requirement.GiftRequirementRuleSystemName,
                    IsLastInGroup = lastRequirement == null || lastRequirement.Id == requirement.Id,
                    InteractionType = groupInteractionType.ToString().ToUpperInvariant()
                };

                var interactionType = requirement.InteractionType ?? RequirementGroupInteractionType.And;
                requirementModel.AvailableInteractionTypes = await interactionType.ToSelectListAsync();

                if (requirement.IsGroup)
                {
                    //get child requirements for the group
                    var childRequirements = await _discountService.GetGiftRequirementsByParentAsync(requirement);

                    requirementModel.ChildRequirements = await PrepareGiftRequirementRuleModelsAsync(childRequirements, discount, interactionType);

                    return requirementModel;
                }

                //or try to get name and configuration URL for the requirement
                var requirementRule = await _discountPluginManager.LoadPluginBySystemNameAsync(requirement.GiftRequirementRuleSystemName);
                if (requirementRule == null)
                    return null;

                requirementModel.RuleName = requirementRule.PluginDescriptor.FriendlyName;
                requirementModel
                    .ConfigurationUrl = requirementRule.GetConfigurationUrl(discount.Id, requirement.Id);

                return requirementModel;
            }).ToListAsync();
        }

        /// <summary>
        /// Prepare paged discount usage history list model
        /// </summary>
        /// <param name="searchModel">Gift usage history search model</param>
        /// <param name="gift">Gift</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the discount usage history list model
        /// </returns>
        public virtual async Task<GiftUsageHistoryListModel> PrepareGiftUsageHistoryListModelAsync(GiftUsageHistorySearchModel searchModel,
            Gift gift)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (gift == null)
                throw new ArgumentNullException(nameof(gift));

            //get discount usage history
            var history = await _discountService.GetAllGiftUsageHistoryAsync(giftId: gift.Id,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new GiftUsageHistoryListModel().PrepareToGridAsync(searchModel, history, () =>
            {
                return history.SelectAwait(async historyEntry =>
                {
                    //fill in model values from the entity
                    var giftUsageHistoryModel = historyEntry.ToModel<GiftUsageHistoryModel>();

                    //convert dates to the user time
                    giftUsageHistoryModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(historyEntry.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    var order = await _orderService.GetOrderByIdAsync(historyEntry.OrderId);
                    if (order != null)
                    {
                        giftUsageHistoryModel.OrderTotal = await _priceFormatter.FormatPriceAsync(order.OrderTotal, true, false);
                        giftUsageHistoryModel.CustomOrderNumber = order.CustomOrderNumber;
                    }

                    return giftUsageHistoryModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged discount product list model
        /// </summary>
        /// <param name="searchModel">Gift product search model</param>
        /// <param name="discount">Gift</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the discount product list model
        /// </returns>
        public virtual async Task<GiftProductListModel> PrepareGiftProductListModelAsync(GiftProductSearchModel searchModel, Gift discount)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (discount == null)
                throw new ArgumentNullException(nameof(discount));

            //get products with applied discount
            var giftProducts = await _productGiftService.GetProductsWithAppliedGiftAsync(giftId: discount.Id,
                showHidden: false,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new GiftProductListModel().PrepareToGrid(searchModel, giftProducts, () =>
            {
                //fill in model values from the entity
                return giftProducts.Select(product =>
                {
                    var discountProductModel = product.ToModel<GiftProductModel>();
                    discountProductModel.ProductId = product.Id;
                    discountProductModel.ProductName = product.Name;

                    return discountProductModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare product search model to add to the discount
        /// </summary>
        /// <param name="searchModel">Product search model to add to the discount</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product search model to add to the discount
        /// </returns>
        public virtual async Task<AddProductToGiftSearchModel> PrepareAddProductToGiftSearchModelAsync(AddProductToGiftSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available categories
            await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.AvailableCategories);

            //prepare available manufacturers
            await _baseAdminModelFactory.PrepareManufacturersAsync(searchModel.AvailableManufacturers);

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            //prepare available vendors
            await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

            //prepare available product types
            await _baseAdminModelFactory.PrepareProductTypesAsync(searchModel.AvailableProductTypes);

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged product list model to add to the discount
        /// </summary>
        /// <param name="searchModel">Product search model to add to the discount</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product list model to add to the discount
        /// </returns>
        public virtual async Task<AddProductToGiftListModel> PrepareAddProductToGiftListModelAsync(AddProductToGiftSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get products
            var products = await _productService.SearchProductsAsync(showHidden: true,
                categoryIds: new List<int> { searchModel.SearchCategoryId },
                manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                productType: searchModel.SearchProductTypeId > 0 ? (ProductType?)searchModel.SearchProductTypeId : null,
                keywords: searchModel.SearchProductName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new AddProductToGiftListModel().PrepareToGridAsync(searchModel, products, () =>
            {
                return products.SelectAwait(async product =>
                {
                    var productModel = product.ToModel<ProductModel>();
                    productModel.SeName = await _urlRecordService.GetSeNameAsync(product, 0, true, false);

                    return productModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged discount category list model
        /// </summary>
        /// <param name="searchModel">Gift category search model</param>
        /// <param name="discount">Gift</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the discount category list model
        /// </returns>
        public virtual async Task<GiftCategoryListModel> PrepareGiftCategoryListModelAsync(GiftCategorySearchModel searchModel, Gift gift)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (gift == null)
                throw new ArgumentNullException(nameof(gift));

            //get categories with applied discount
            var giftCategories = await _categoryGiftService.GetCategoriesByAppliedGiftAsync(giftId: gift.Id,
                showHidden: false,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new GiftCategoryListModel().PrepareToGridAsync(searchModel, giftCategories, () =>
            {
                //fill in model values from the entity
                return giftCategories.SelectAwait(async category =>
                {
                    var giftCategoryModel = category.ToModel<GiftCategoryModel>();

                    giftCategoryModel.CategoryName = await _categoryService.GetFormattedBreadCrumbAsync(category);
                    giftCategoryModel.CategoryId = category.Id;

                    return giftCategoryModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare category search model to add to the discount
        /// </summary>
        /// <param name="searchModel">Category search model to add to the discount</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category search model to add to the discount
        /// </returns>
        public virtual Task<AddCategoryToGiftSearchModel> PrepareAddCategoryToGiftSearchModelAsync(AddCategoryToGiftSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged category list model to add to the discount
        /// </summary>
        /// <param name="searchModel">Category search model to add to the discount</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category list model to add to the discount
        /// </returns>
        public virtual async Task<AddCategoryToGiftListModel> PrepareAddCategoryToGiftListModelAsync(AddCategoryToGiftSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get categories
            var categories = await _categoryService.GetAllCategoriesAsync(showHidden: true,
                categoryName: searchModel.SearchCategoryName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new AddCategoryToGiftListModel().PrepareToGridAsync(searchModel, categories, () =>
            {
                return categories.SelectAwait(async category =>
                {
                    //fill in model values from the entity
                    var categoryModel = category.ToModel<CategoryModel>();

                    //fill in additional values (not existing in the entity)
                    categoryModel.Breadcrumb = await _categoryService.GetFormattedBreadCrumbAsync(category);
                    categoryModel.SeName = await _urlRecordService.GetSeNameAsync(category, 0, true, false);

                    return categoryModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged discount manufacturer list model
        /// </summary>
        /// <param name="searchModel">Gift manufacturer search model</param>
        /// <param name="discount">Gift</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the discount manufacturer list model
        /// </returns>
        public virtual async Task<GiftManufacturerListModel> PrepareGiftManufacturerListModelAsync(GiftManufacturerSearchModel searchModel,
            Gift gift)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (gift == null)
                throw new ArgumentNullException(nameof(gift));

            //get manufacturers with applied discount
            var giftManufacturers = await _manufacturerGiftService.GetManufacturersWithAppliedGiftAsync(giftId: gift.Id,
                showHidden: false,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new GiftManufacturerListModel().PrepareToGrid(searchModel, giftManufacturers, () =>
            {
                //fill in model values from the entity
                return giftManufacturers.Select(manufacturer =>
                {
                    var giftManufacturerModel = manufacturer.ToModel<GiftManufacturerModel>();
                    giftManufacturerModel.ManufacturerId = manufacturer.Id;
                    giftManufacturerModel.ManufacturerName = manufacturer.Name;

                    return giftManufacturerModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare manufacturer search model to add to the discount
        /// </summary>
        /// <param name="searchModel">Manufacturer search model to add to the discount</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the manufacturer search model to add to the discount
        /// </returns>
        public virtual Task<AddManufacturerToGiftSearchModel> PrepareAddManufacturerToGiftSearchModelAsync(AddManufacturerToGiftSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return Task.FromResult(searchModel);
        }

        /// <summary>
        /// Prepare paged manufacturer list model to add to the discount
        /// </summary>
        /// <param name="searchModel">Manufacturer search model to add to the discount</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the manufacturer list model to add to the discount
        /// </returns>
        public virtual async Task<AddManufacturerToGiftListModel> PrepareAddManufacturerToGiftListModelAsync(AddManufacturerToGiftSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get manufacturers
            var manufacturers = await _manufacturerService.GetAllManufacturersAsync(showHidden: true,
                manufacturerName: searchModel.SearchManufacturerName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new AddManufacturerToGiftListModel().PrepareToGridAsync(searchModel, manufacturers, () =>
            {
                return manufacturers.SelectAwait(async manufacturer =>
                {
                    var manufacturerModel = manufacturer.ToModel<ManufacturerModel>();
                    manufacturerModel.SeName = await _urlRecordService.GetSeNameAsync(manufacturer, 0, true, false);

                    return manufacturerModel;
                });
            });

            return model;
        }

        #endregion
    }
}