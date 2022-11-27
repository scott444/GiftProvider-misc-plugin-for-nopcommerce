using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Plugin.Misc.GiftProvider.Areas.Admin.Models.Gifts;
using Nop.Plugin.Misc.GiftProvider.Domain;

namespace Nop.Plugin.Misc.GiftProvider.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the discount model factory
    /// </summary>
    public partial interface IGiftModelFactory
    {
        /// <summary>
        /// Prepare discount search model
        /// </summary>
        /// <param name="searchModel">Gift search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the discount search model
        /// </returns>
        Task<GiftSearchModel> PrepareGiftSearchModelAsync(GiftSearchModel searchModel);

        /// <summary>
        /// Prepare paged discount list model
        /// </summary>
        /// <param name="searchModel">Gift search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the discount list model
        /// </returns>
        Task<GiftListModel> PrepareGiftListModelAsync(GiftSearchModel searchModel);

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
        Task<GiftModel> PrepareGiftModelAsync(GiftModel model, Gift discount, bool excludeProperties = false);

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
        Task<IList<GiftRequirementRuleModel>> PrepareGiftRequirementRuleModelsAsync(ICollection<GiftRequirement> requirements,
            Gift discount, RequirementGroupInteractionType groupInteractionType);

        /// <summary>
        /// Prepare paged discount usage history list model
        /// </summary>
        /// <param name="searchModel">Gift usage history search model</param>
        /// <param name="discount">Gift</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the discount usage history list model
        /// </returns>
        Task<GiftUsageHistoryListModel> PrepareGiftUsageHistoryListModelAsync(GiftUsageHistorySearchModel searchModel,
            Gift discount);

        /// <summary>
        /// Prepare paged discount product list model
        /// </summary>
        /// <param name="searchModel">Gift product search model</param>
        /// <param name="discount">Gift</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the discount product list model
        /// </returns>
        Task<GiftProductListModel> PrepareGiftProductListModelAsync(GiftProductSearchModel searchModel, Gift discount);

        /// <summary>
        /// Prepare product search model to add to the discount
        /// </summary>
        /// <param name="searchModel">Product search model to add to the discount</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product search model to add to the discount
        /// </returns>
        Task<AddProductToGiftSearchModel> PrepareAddProductToGiftSearchModelAsync(AddProductToGiftSearchModel searchModel);

        /// <summary>
        /// Prepare paged product list model to add to the discount
        /// </summary>
        /// <param name="searchModel">Product search model to add to the discount</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the product list model to add to the discount
        /// </returns>
        Task<AddProductToGiftListModel> PrepareAddProductToGiftListModelAsync(AddProductToGiftSearchModel searchModel);

        /// <summary>
        /// Prepare paged discount category list model
        /// </summary>
        /// <param name="searchModel">Gift category search model</param>
        /// <param name="discount">Gift</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the discount category list model
        /// </returns>
        Task<GiftCategoryListModel> PrepareGiftCategoryListModelAsync(GiftCategorySearchModel searchModel, Gift discount);

        /// <summary>
        /// Prepare category search model to add to the discount
        /// </summary>
        /// <param name="searchModel">Category search model to add to the discount</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category search model to add to the discount
        /// </returns>
        Task<AddCategoryToGiftSearchModel> PrepareAddCategoryToGiftSearchModelAsync(AddCategoryToGiftSearchModel searchModel);

        /// <summary>
        /// Prepare paged category list model to add to the discount
        /// </summary>
        /// <param name="searchModel">Category search model to add to the discount</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category list model to add to the discount
        /// </returns>
        Task<AddCategoryToGiftListModel> PrepareAddCategoryToGiftListModelAsync(AddCategoryToGiftSearchModel searchModel);

        /// <summary>
        /// Prepare paged discount manufacturer list model
        /// </summary>
        /// <param name="searchModel">Gift manufacturer search model</param>
        /// <param name="discount">Gift</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the discount manufacturer list model
        /// </returns>
        Task<GiftManufacturerListModel> PrepareGiftManufacturerListModelAsync(GiftManufacturerSearchModel searchModel,
            Gift discount);

        /// <summary>
        /// Prepare manufacturer search model to add to the discount
        /// </summary>
        /// <param name="searchModel">Manufacturer search model to add to the discount</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the manufacturer search model to add to the discount
        /// </returns>
        Task<AddManufacturerToGiftSearchModel> PrepareAddManufacturerToGiftSearchModelAsync(AddManufacturerToGiftSearchModel searchModel);

        /// <summary>
        /// Prepare paged manufacturer list model to add to the discount
        /// </summary>
        /// <param name="searchModel">Manufacturer search model to add to the discount</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the manufacturer list model to add to the discount
        /// </returns>
        Task<AddManufacturerToGiftListModel> PrepareAddManufacturerToGiftListModelAsync(AddManufacturerToGiftSearchModel searchModel);
    }
}