using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.GiftProvider.Domain;

namespace Nop.Plugin.Misc.GiftProvider.Services;

public partial interface IProductGiftService
{
    Task ClearGiftProductMappingAsync(Gift gift);
    Task DeleteGiftProductMappingAsync(GiftProductMapping giftProductMapping);
    Task<GiftProductMapping> GetGiftAppliedToProductAsync(int giftId, int productId);
    Task<IPagedList<Product>> GetProductsWithAppliedGiftAsync(int? giftId, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);

    Task InsertGiftProductMappingAsync(GiftProductMapping giftProductMapping);
    Task UpdateHasGiftsAppliedAsync(Product product, bool hasGiftsApplied);
}
