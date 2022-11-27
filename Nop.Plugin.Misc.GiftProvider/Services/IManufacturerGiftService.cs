using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.GiftProvider.Domain;

namespace Nop.Plugin.Misc.GiftProvider.Services;

public partial interface IManufacturerGiftService
{
    Task ClearGiftManufacturerMappingAsync(Gift gift);
    Task DeleteGiftManufacturerMappingAsync(GiftManufacturerMapping giftManufacturerMapping);
    Task<GiftManufacturerMapping> GetGiftAppliedToManufacturerAsync(int manufacturerId, int giftId);
    Task<IPagedList<Manufacturer>> GetManufacturersWithAppliedGiftAsync(int? giftId, bool showHidden=false, int pageIndex=0, int pageSize=int.MaxValue);
    Task InsertGiftManufacturerMappingAsync(GiftManufacturerMapping giftManufacturerMapping);
}
