using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.GiftProvider.Domain;

namespace Nop.Plugin.Misc.GiftProvider.Services;

public partial interface ICategoryGiftService
{
    Task ClearGiftCategoryMappingAsync(Gift gift);
    Task DeleteGiftCategoryMappingAsync(GiftCategoryMapping mapping);
    Task<IPagedList<Category>> GetCategoriesByAppliedGiftAsync(int? giftId=null, bool showHidden = false, int pageIndex=0, int pageSize=int.MaxValue);
    Task<GiftCategoryMapping> GetGiftAppliedToCategoryAsync(int categoryId, int giftId);
    Task InsertGiftCategoryMappingAsync(GiftCategoryMapping giftCategoryMapping);
}
