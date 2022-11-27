using System;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Plugin.Misc.GiftProvider.Domain;

namespace Nop.Plugin.Misc.GiftProvider.Services;

public class ManufacturerGiftService : IManufacturerGiftService
{
    private readonly IRepository<GiftManufacturerMapping> _giftManufacturerMappingRepository;
    private readonly IRepository<Manufacturer> _manufacturerRepository;

    public async Task ClearGiftManufacturerMappingAsync(Gift gift)
    {
        if (gift is null)
            throw new ArgumentNullException(nameof(gift));

        var mappings = _giftManufacturerMappingRepository.Table.Where(dcm => dcm.GiftId == gift.Id);

        await _giftManufacturerMappingRepository.DeleteAsync(mappings.ToList());
    }

    public async Task DeleteGiftManufacturerMappingAsync(GiftManufacturerMapping giftManufacturerMapping)
    {
        await _giftManufacturerMappingRepository.DeleteAsync(giftManufacturerMapping);
    }

    public async  Task<GiftManufacturerMapping> GetGiftAppliedToManufacturerAsync(int manufacturerId, int giftId)
    {
        return await _giftManufacturerMappingRepository.Table
             .FirstOrDefaultAsync(dcm => dcm.EntityId == manufacturerId && dcm.GiftId== giftId);
    }

    public async Task<IPagedList<Manufacturer>> GetManufacturersWithAppliedGiftAsync(int? giftId, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var manufacturers = _manufacturerRepository.Table;

        if (giftId.HasValue)
            manufacturers = from manufacturer in manufacturers
                            join dmm in _giftManufacturerMappingRepository.Table on manufacturer.Id equals dmm.EntityId
                            where dmm.GiftId == giftId.Value
                            select manufacturer;

        if (!showHidden)
            manufacturers = manufacturers.Where(manufacturer => !manufacturer.Deleted);

        manufacturers = manufacturers.OrderBy(manufacturer => manufacturer.DisplayOrder).ThenBy(manufacturer => manufacturer.Id);

        return await manufacturers.ToPagedListAsync(pageIndex, pageSize);
    }

    public async Task InsertGiftManufacturerMappingAsync(GiftManufacturerMapping giftManufacturerMapping)
    {
        await _giftManufacturerMappingRepository.InsertAsync(giftManufacturerMapping);
    }
}