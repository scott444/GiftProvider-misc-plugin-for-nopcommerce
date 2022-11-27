using System;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Plugin.Misc.GiftProvider.Domain;

namespace Nop.Plugin.Misc.GiftProvider.Services;

public class CategoryGiftService : ICategoryGiftService
{
    private readonly IRepository<GiftCategoryMapping> _giftCategoryMappingRepository;
    private readonly IRepository<Category> _categoryRepository;


    public async Task ClearGiftCategoryMappingAsync(Gift gift)
    {
        if (gift is null)
            throw new ArgumentNullException(nameof(gift));

        var mappings = _giftCategoryMappingRepository.Table.Where(dcm => dcm.GiftId == gift.Id);

        await _giftCategoryMappingRepository.DeleteAsync(mappings.ToList());
    }

    public async Task DeleteGiftCategoryMappingAsync(GiftCategoryMapping mapping)
    {
        await _giftCategoryMappingRepository.DeleteAsync(mapping);
    }

    public async Task<IPagedList<Category>> GetCategoriesByAppliedGiftAsync(int? giftId = null, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var categories = _categoryRepository.Table;

        if (giftId.HasValue)
            categories = from category in categories
                         join dcm in _giftCategoryMappingRepository.Table on category.Id equals dcm.EntityId
                         where dcm.GiftId == giftId.Value
                         select category;

        if (!showHidden)
            categories = categories.Where(category => !category.Deleted);

        categories = categories.OrderBy(category => category.DisplayOrder).ThenBy(category => category.Id);

        return await categories.ToPagedListAsync(pageIndex, pageSize);
    }

    public async Task<GiftCategoryMapping> GetGiftAppliedToCategoryAsync(int categoryId, int giftId)
    {
        return await _giftCategoryMappingRepository.Table
                  .FirstOrDefaultAsync(gcm => gcm.EntityId == categoryId && gcm.GiftId == giftId);
    }

    public async Task InsertGiftCategoryMappingAsync(GiftCategoryMapping giftCategoryMapping)
    {
        await _giftCategoryMappingRepository.InsertAsync(giftCategoryMapping);

    }
}