using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.GiftProvider.Areas.Admin.Factories;
using Nop.Plugin.Misc.GiftProvider.Areas.Admin.Models.Gifts;
using Nop.Plugin.Misc.GiftProvider.Domain;
using Nop.Plugin.Misc.GiftProvider.Services;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.GiftProvider.Areas.Admin.Controllers
{
    public partial class GiftController : BaseAdminController
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly ICategoryGiftService _categoryGiftService;
        private readonly ICategoryService _categoryService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IGiftModelFactory _giftModelFactory;
        private readonly IGiftPluginManager _giftPluginManager;
        private readonly IGiftService _giftService;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerGiftService _manufacturerGiftService;
        private readonly IManufacturerService _manufacturerService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IProductGiftService _productGiftService;
        private readonly IProductService _productService;

        #endregion

        #region Ctor

        public GiftController(CatalogSettings catalogSettings,
            ICategoryGiftService categoryGiftService,
            ICategoryService categoryService,
            ICustomerActivityService customerActivityService,
            IGiftModelFactory giftModelFactory,
            IGiftPluginManager giftPluginManager,
            IGiftService giftService,
            ILocalizationService localizationService,
            IManufacturerGiftService manufacturerGiftService,
            IManufacturerService manufacturerService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IProductGiftService productGiftService)
        {
            _catalogSettings = catalogSettings;
            _categoryService = categoryService;
            _categoryGiftService = categoryGiftService;
            _customerActivityService = customerActivityService;
            _giftModelFactory = giftModelFactory;
            _giftPluginManager = giftPluginManager;
            _giftService = giftService;
            _localizationService = localizationService;
            _manufacturerGiftService = manufacturerGiftService;
            _manufacturerService= manufacturerService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _productGiftService = productGiftService;
        }

        #endregion

        #region Methods

        #region Gifts

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(GiftPermissionProvider.ManageGifts))
                return AccessDeniedView();

            //whether gifts are ignored
            //if (_catalogSettings.IgnoreGifts)
            //    _notificationService.WarningNotification(await _localizationService.GetResourceAsync("Admin.Promotions.Gifts.IgnoreGifts.Warning"));

            //prepare model
            var model = await _giftModelFactory.PrepareGiftSearchModelAsync(new GiftSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(GiftSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(GiftPermissionProvider.ManageGifts))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _giftModelFactory.PrepareGiftListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(GiftPermissionProvider.ManageGifts))
                return AccessDeniedView();

            //prepare model
            var model = await _giftModelFactory.PrepareGiftModelAsync(new GiftModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(GiftModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(GiftPermissionProvider.ManageGifts))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var gift = model.ToEntity<Gift>();
                await _giftService.InsertGiftAsync(gift);

                //activity log
                await _customerActivityService.InsertActivityAsync("AddNewGift",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewGift"), gift.Name), gift);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.Gifts.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = gift.Id });
            }

            //prepare model
            model = await _giftModelFactory.PrepareGiftModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(GiftPermissionProvider.ManageGifts))
                return AccessDeniedView();

            //try to get a gift with the specified id
            var gift = await _giftService.GetGiftByIdAsync(id);
            if (gift == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _giftModelFactory.PrepareGiftModelAsync(null, gift);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(GiftModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(GiftPermissionProvider.ManageGifts))
                return AccessDeniedView();

            //try to get a gift with the specified id
            var gift = await _giftService.GetGiftByIdAsync(model.Id);
            if (gift == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var prevGiftType = gift.GiftType;
                gift = model.ToEntity(gift);
                await _giftService.UpdateGiftAsync(gift);

                //clean up old references (if changed) 
                if (prevGiftType != gift.GiftType)
                {
                    switch (prevGiftType)
                    {
                        case GiftType.AssignedToSkus:
                            await _productGiftService.ClearGiftProductMappingAsync(gift);
                            break;
                        case GiftType.AssignedToCategories:
                            await _categoryGiftService.ClearGiftCategoryMappingAsync(gift);
                            break;
                        case GiftType.AssignedToManufacturers:
                            await _manufacturerGiftService.ClearGiftManufacturerMappingAsync(gift);
                            break;
                        default:
                            break;
                    }
                }

                //activity log
                await _customerActivityService.InsertActivityAsync("EditGift",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditGift"), gift.Name), gift);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.Gifts.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = gift.Id });
            }

            //prepare model
            model = await _giftModelFactory.PrepareGiftModelAsync(model, gift, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(GiftPermissionProvider.ManageGifts))
                return AccessDeniedView();

            //try to get a gift with the specified id
            var gift = await _giftService.GetGiftByIdAsync(id);
            if (gift == null)
                return RedirectToAction("List");

            //applied to products
            var products = await _productGiftService.GetProductsWithAppliedGiftAsync(gift.Id, true);

            await _giftService.DeleteGiftAsync(gift);

            //update "HasGiftsApplied" properties
            foreach (var p in products)
                await _productGiftService.UpdateHasGiftsAppliedAsync(p, hasGiftsApplied: false);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteGift",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteGift"), gift.Name), gift);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Promotions.Gifts.Deleted"));

            return RedirectToAction("List");
        }

        #endregion

        #region Gift requirements

        public virtual async Task<IActionResult> GetGiftRequirementConfigurationUrl(string systemName, int giftId, int? giftRequirementId)
        {
            if (!await _permissionService.AuthorizeAsync(GiftPermissionProvider.ManageGifts))
                return AccessDeniedView();

            if (string.IsNullOrEmpty(systemName))
                throw new ArgumentNullException(nameof(systemName));

            var giftRequirementRule = await _giftPluginManager.LoadPluginBySystemNameAsync(systemName)
                ?? throw new ArgumentException("Gift requirement rule could not be loaded");

            var gift = await _giftService.GetGiftByIdAsync(giftId)
                ?? throw new ArgumentException("Gift could not be loaded");

            var url = giftRequirementRule.GetConfigurationUrl(gift.Id, giftRequirementId);

            return Json(new { url });
        }

        public virtual async Task<IActionResult> GetGiftRequirements(int giftId, int giftRequirementId,
            int? parentId, int? interactionTypeId, bool deleteRequirement)
        {
            if (!await _permissionService.AuthorizeAsync(GiftPermissionProvider.ManageGifts))
                return AccessDeniedView();

            var requirements = new List<GiftRequirementRuleModel>();

            var gift = await _giftService.GetGiftByIdAsync(giftId);
            if (gift == null)
                return Json(requirements);

            var giftRequirement = await _giftService.GetGiftRequirementByIdAsync(giftRequirementId);
            if (giftRequirement != null)
            {
                //delete
                if (deleteRequirement)
                {
                    await _giftService.DeleteGiftRequirementAsync(giftRequirement, true);

                    var giftRequirements = await _giftService.GetAllGiftRequirementsAsync(gift.Id);

                    //delete default group if there are no any requirements
                    if (!giftRequirements.Any(requirement => requirement.ParentId.HasValue))
                    {
                        foreach (var dr in giftRequirements)
                            await _giftService.DeleteGiftRequirementAsync(dr, true);
                    }
                }
                //or update the requirement
                else
                {
                    var defaultGroupId = (await _giftService.GetAllGiftRequirementsAsync(gift.Id, true)).FirstOrDefault(requirement => requirement.IsGroup)?.Id ?? 0;
                    if (defaultGroupId == 0)
                    {
                        //add default requirement group
                        var defaultGroup = new GiftRequirement
                        {
                            IsGroup = true,
                            GiftId = gift.Id,
                            InteractionType = RequirementGroupInteractionType.And,
                            GiftRequirementRuleSystemName = await _localizationService
                                .GetResourceAsync("Admin.Promotions.Gifts.Requirements.DefaultRequirementGroup")
                        };

                        await _giftService.InsertGiftRequirementAsync(defaultGroup);

                        defaultGroupId = defaultGroup.Id;
                    }

                    //set parent identifier if specified
                    if (parentId.HasValue)
                        giftRequirement.ParentId = parentId.Value;
                    else
                    {
                        //or default group identifier
                        if (defaultGroupId != giftRequirement.Id)
                            giftRequirement.ParentId = defaultGroupId;
                    }

                    //set interaction type
                    if (interactionTypeId.HasValue)
                        giftRequirement.InteractionTypeId = interactionTypeId;

                    await _giftService.UpdateGiftRequirementAsync(giftRequirement);
                }
            }

            //get current requirements
            var topLevelRequirements = (await _giftService.GetAllGiftRequirementsAsync(gift.Id, true)).Where(requirement => requirement.IsGroup).ToList();

            //get interaction type of top-level group
            var interactionType = topLevelRequirements.FirstOrDefault()?.InteractionType;

            if (interactionType.HasValue)
            {
                requirements = (await _giftModelFactory
                    .PrepareGiftRequirementRuleModelsAsync(topLevelRequirements, gift, interactionType.Value)).ToList();
            }

            //get available groups
            var requirementGroups = (await _giftService.GetAllGiftRequirementsAsync(gift.Id)).Where(requirement => requirement.IsGroup);

            var availableRequirementGroups = requirementGroups.Select(requirement =>
                new SelectListItem { Value = requirement.Id.ToString(), Text = requirement.GiftRequirementRuleSystemName }).ToList();

            return Json(new { Requirements = requirements, AvailableGroups = availableRequirementGroups });
        }

        public virtual async Task<IActionResult> AddNewGroup(int giftId, string name)
        {
            if (!await _permissionService.AuthorizeAsync(GiftPermissionProvider.ManageGifts))
                return AccessDeniedView();

            var gift = await _giftService.GetGiftByIdAsync(giftId);
            if (gift == null)
                throw new ArgumentException("Gift could not be loaded");

            var defaultGroup = (await _giftService.GetAllGiftRequirementsAsync(gift.Id, true)).FirstOrDefault(requirement => requirement.IsGroup);
            if (defaultGroup == null)
            {
                //add default requirement group
                await _giftService.InsertGiftRequirementAsync(new GiftRequirement
                {
                    GiftId = gift.Id,
                    IsGroup = true,
                    InteractionType = RequirementGroupInteractionType.And,
                    GiftRequirementRuleSystemName = await _localizationService
                        .GetResourceAsync("Admin.Promotions.Gifts.Requirements.DefaultRequirementGroup")
                });
            }

            //save new requirement group
            var giftRequirementGroup = new GiftRequirement
            {
                GiftId = gift.Id,
                IsGroup = true,
                GiftRequirementRuleSystemName = name,
                InteractionType = RequirementGroupInteractionType.And
            };

            await _giftService.InsertGiftRequirementAsync(giftRequirementGroup);

            if (!string.IsNullOrEmpty(name))
                return Json(new { Result = true, NewRequirementId = giftRequirementGroup.Id });

            //set identifier as group name (if not specified)
            giftRequirementGroup.GiftRequirementRuleSystemName = $"#{giftRequirementGroup.Id}";
            await _giftService.UpdateGiftRequirementAsync(giftRequirementGroup);

            await _giftService.UpdateGiftAsync(gift);

            return Json(new { Result = true, NewRequirementId = giftRequirementGroup.Id });
        }

        #endregion

        #region Applied to products

        [HttpPost]
        public virtual async Task<IActionResult> ProductList(GiftProductSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(GiftPermissionProvider.ManageGifts))
                return await AccessDeniedDataTablesJson();

            //try to get a gift with the specified id
            var gift = await _giftService.GetGiftByIdAsync(searchModel.GiftId)
                ?? throw new ArgumentException("No gift found with the specified id");

            //prepare model
            var model = await _giftModelFactory.PrepareGiftProductListModelAsync(searchModel, gift);

            return Json(model);
        }

        public virtual async Task<IActionResult> ProductDelete(int giftId, int productId)
        {
            if (!await _permissionService.AuthorizeAsync(GiftPermissionProvider.ManageGifts))
                return AccessDeniedView();

            //try to get a gift with the specified id
            var gift = await _giftService.GetGiftByIdAsync(giftId)
                ?? throw new ArgumentException("No gift found with the specified id", nameof(giftId));

            //try to get a product with the specified id
            var product = await _productService.GetProductByIdAsync(productId)
                ?? throw new ArgumentException("No product found with the specified id", nameof(productId));

            //remove gift
            if (await _productGiftService.GetGiftAppliedToProductAsync(product.Id, gift.Id) is GiftProductMapping giftProductMapping)
                await _productGiftService.DeleteGiftProductMappingAsync(giftProductMapping);

            await _productService.UpdateProductAsync(product);
            await _productGiftService.UpdateHasGiftsAppliedAsync(product, hasGiftsApplied: false);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> ProductAddPopup(int giftId)
        {
            if (!await _permissionService.AuthorizeAsync(GiftPermissionProvider.ManageGifts))
                return AccessDeniedView();

            //prepare model
            var model = await _giftModelFactory.PrepareAddProductToGiftSearchModelAsync(new AddProductToGiftSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductAddPopupList(AddProductToGiftSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(GiftPermissionProvider.ManageGifts))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _giftModelFactory.PrepareAddProductToGiftListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> ProductAddPopup(AddProductToGiftModel model)
        {
            if (!await _permissionService.AuthorizeAsync(GiftPermissionProvider.ManageGifts))
                return AccessDeniedView();

            //try to get a gift with the specified id
            var gift = await _giftService.GetGiftByIdAsync(model.GiftId)
                ?? throw new ArgumentException("No gift found with the specified id");

            var selectedProducts = await _productService.GetProductsByIdsAsync(model.SelectedProductIds.ToArray());
            if (selectedProducts.Any())
            {
                foreach (var product in selectedProducts)
                {
                    if (await _productGiftService.GetGiftAppliedToProductAsync(product.Id, gift.Id) is null)
                        await _productGiftService.InsertGiftProductMappingAsync(new GiftProductMapping { EntityId = product.Id, GiftId = gift.Id });

                    await _productService.UpdateProductAsync(product);
                    await _productGiftService.UpdateHasGiftsAppliedAsync(product, hasGiftsApplied: true);
                }
            }

            ViewBag.RefreshPage = true;

            return View(new AddProductToGiftSearchModel());
        }

        #endregion

        #region Applied to categories

        [HttpPost]
        public virtual async Task<IActionResult> CategoryList(GiftCategorySearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(GiftPermissionProvider.ManageGifts))
                return await AccessDeniedDataTablesJson();

            //try to get a gift with the specified id
            var gift = await _giftService.GetGiftByIdAsync(searchModel.GiftId)
                ?? throw new ArgumentException("No gift found with the specified id");

            //prepare model
            var model = await _giftModelFactory.PrepareGiftCategoryListModelAsync(searchModel, gift);

            return Json(model);
        }

        public virtual async Task<IActionResult> CategoryDelete(int giftId, int categoryId)
        {
            if (!await _permissionService.AuthorizeAsync(GiftPermissionProvider.ManageGifts))
                return AccessDeniedView();

            //try to get a gift with the specified id
            var gift = await _giftService.GetGiftByIdAsync(giftId)
                ?? throw new ArgumentException("No gift found with the specified id", nameof(giftId));

            //try to get a category with the specified id
            var category = await _categoryService.GetCategoryByIdAsync(categoryId)
                ?? throw new ArgumentException("No category found with the specified id", nameof(categoryId));

            //remove gift
            if (await _categoryGiftService.GetGiftAppliedToCategoryAsync(category.Id, gift.Id) is GiftCategoryMapping mapping)
                await _categoryGiftService.DeleteGiftCategoryMappingAsync(mapping);

            await _categoryService.UpdateCategoryAsync(category);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> CategoryAddPopup(int giftId)
        {
            if (!await _permissionService.AuthorizeAsync(GiftPermissionProvider.ManageGifts))
                return AccessDeniedView();

            //prepare model
            var model = await _giftModelFactory.PrepareAddCategoryToGiftSearchModelAsync(new AddCategoryToGiftSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CategoryAddPopupList(AddCategoryToGiftSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(GiftPermissionProvider.ManageGifts))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _giftModelFactory.PrepareAddCategoryToGiftListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> CategoryAddPopup(AddCategoryToGiftModel model)
        {
            if (!await _permissionService.AuthorizeAsync(GiftPermissionProvider.ManageGifts))
                return AccessDeniedView();

            //try to get a gift with the specified id
            var gift = await _giftService.GetGiftByIdAsync(model.GiftId)
                ?? throw new ArgumentException("No gift found with the specified id");

            foreach (var id in model.SelectedCategoryIds)
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                if (category == null)
                    continue;

                if (await _categoryGiftService.GetGiftAppliedToCategoryAsync(category.Id, gift.Id) is null)
                    await _categoryGiftService.InsertGiftCategoryMappingAsync(new GiftCategoryMapping { GiftId = gift.Id, EntityId = category.Id });

                await _categoryService.UpdateCategoryAsync(category);
            }

            ViewBag.RefreshPage = true;

            return View(new AddCategoryToGiftSearchModel());
        }

        #endregion

        #region Applied to manufacturers

        [HttpPost]
        public virtual async Task<IActionResult> ManufacturerList(GiftManufacturerSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(GiftPermissionProvider.ManageGifts))
                return await AccessDeniedDataTablesJson();

            //try to get a gift with the specified id
            var gift = await _giftService.GetGiftByIdAsync(searchModel.GiftId)
                ?? throw new ArgumentException("No gift found with the specified id");

            //prepare model
            var model = await _giftModelFactory.PrepareGiftManufacturerListModelAsync(searchModel, gift);

            return Json(model);
        }

        public virtual async Task<IActionResult> ManufacturerDelete(int giftId, int manufacturerId)
        {
            if (!await _permissionService.AuthorizeAsync(GiftPermissionProvider.ManageGifts))
                return AccessDeniedView();

            //try to get a gift with the specified id
            var gift = await _giftService.GetGiftByIdAsync(giftId)
                ?? throw new ArgumentException("No gift found with the specified id", nameof(giftId));

            //try to get a manufacturer with the specified id
            var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(manufacturerId)
                ?? throw new ArgumentException("No manufacturer found with the specified id", nameof(manufacturerId));

            //remove gift
            if (await _manufacturerGiftService.GetGiftAppliedToManufacturerAsync(manufacturer.Id, gift.Id) is GiftManufacturerMapping giftManufacturerMapping)
                await _manufacturerGiftService.DeleteGiftManufacturerMappingAsync(giftManufacturerMapping);

            await _manufacturerService.UpdateManufacturerAsync(manufacturer);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> ManufacturerAddPopup(int giftId)
        {
            if (!await _permissionService.AuthorizeAsync(GiftPermissionProvider.ManageGifts))
                return AccessDeniedView();

            //prepare model
            var model = await _giftModelFactory.PrepareAddManufacturerToGiftSearchModelAsync(new AddManufacturerToGiftSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ManufacturerAddPopupList(AddManufacturerToGiftSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(GiftPermissionProvider.ManageGifts))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _giftModelFactory.PrepareAddManufacturerToGiftListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> ManufacturerAddPopup(AddManufacturerToGiftModel model)
        {
            if (!await _permissionService.AuthorizeAsync(GiftPermissionProvider.ManageGifts))
                return AccessDeniedView();

            //try to get a gift with the specified id
            var gift = await _giftService.GetGiftByIdAsync(model.GiftId)
                ?? throw new ArgumentException("No gift found with the specified id");

            foreach (var id in model.SelectedManufacturerIds)
            {
                var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(id);
                if (manufacturer == null)
                    continue;

                if (await _manufacturerGiftService.GetGiftAppliedToManufacturerAsync(manufacturer.Id, gift.Id) is null)
                    await _manufacturerGiftService.InsertGiftManufacturerMappingAsync(new GiftManufacturerMapping { EntityId = manufacturer.Id, GiftId = gift.Id });

                await _manufacturerService.UpdateManufacturerAsync(manufacturer);
            }

            ViewBag.RefreshPage = true;

            return View(new AddManufacturerToGiftSearchModel());
        }

        #endregion

        #region Gift usage history

        [HttpPost]
        public virtual async Task<IActionResult> UsageHistoryList(GiftUsageHistorySearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(GiftPermissionProvider.ManageGifts))
                return await AccessDeniedDataTablesJson();

            //try to get a gift with the specified id
            var gift = await _giftService.GetGiftByIdAsync(searchModel.GiftId)
                ?? throw new ArgumentException("No gift found with the specified id");

            //prepare model
            var model = await _giftModelFactory.PrepareGiftUsageHistoryListModelAsync(searchModel, gift);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> UsageHistoryDelete(int giftId, int id)
        {
            if (!await _permissionService.AuthorizeAsync(GiftPermissionProvider.ManageGifts))
                return AccessDeniedView();

            //try to get a gift with the specified id
            _ = await _giftService.GetGiftByIdAsync(giftId)
                ?? throw new ArgumentException("No gift found with the specified id", nameof(giftId));

            //try to get a gift usage history entry with the specified id
            var giftUsageHistoryEntry = await _giftService.GetGiftUsageHistoryByIdAsync(id)
                ?? throw new ArgumentException("No gift usage history entry found with the specified id", nameof(id));

            await _giftService.DeleteGiftUsageHistoryAsync(giftUsageHistoryEntry);

            return new NullJsonResult();
        }

        #endregion

        #endregion
    }
}