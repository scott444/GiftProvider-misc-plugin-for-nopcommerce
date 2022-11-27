using System;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Plugin.Misc.GiftProvider.Domain;
using Nop.Services.Common;

namespace Nop.Plugin.Misc.GiftProvider.Services;

public class ProductGiftService : IProductGiftService
{
    private readonly IRepository<GiftProductMapping> _giftProductMappingRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly IGenericAttributeService _genericAttributeService;

    public ProductGiftService(
        IRepository<GiftProductMapping> giftProductMappingRepository,
        IRepository<Product> productRepository,
        IGenericAttributeService genericAttributeService)
    {
        _giftProductMappingRepository = giftProductMappingRepository;
        _productRepository = productRepository;
        _genericAttributeService = genericAttributeService;
    }

    public async Task ClearGiftProductMappingAsync(Gift gift)
    {
        if (gift is null)
            throw new ArgumentNullException(nameof(gift));

        var mappingsWithProducts =
            from dcm in _giftProductMappingRepository.Table
            join p in _productRepository.Table on dcm.EntityId equals p.Id
            where dcm.GiftId == gift.Id
            select new { product = p, dcm };

        foreach (var pdcm in await mappingsWithProducts.ToListAsync())
        {
            await _giftProductMappingRepository.DeleteAsync(pdcm.dcm);

            //update "HasDiscountsApplied" property
            await UpdateHasGiftsAppliedAsync(pdcm.product);
        }
    }

    public virtual async Task UpdateHasGiftsAppliedAsync(Product product)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));


        var hasGiftssApplied = _giftProductMappingRepository.Table.Any(dpm => dpm.EntityId == product.Id);
        await UpdateProductAsync(product, hasGiftssApplied);
    }

    public virtual async Task UpdateHasGiftsAppliedAsync(Product product, bool hasGiftsApplied)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));
        
        await UpdateProductAsync(product, hasGiftsApplied);
    }

    public virtual async Task UpdateProductAsync(Product product, bool hasGiftsApplied)
    {
        var productAttributes = await _genericAttributeService.GetAttributesForEntityAsync(product.Id, "Product");
        var attribute = productAttributes.FirstOrDefault(x => x.Key == "HasGiftsApplied");
        if (attribute == null)
        {
            attribute = new Core.Domain.Common.GenericAttribute
            {
                EntityId = product.Id,
                KeyGroup = "Product",
                Key = "HasGiftsApplied",
                Value = hasGiftsApplied.ToString()
            };
            await _genericAttributeService.InsertAttributeAsync(attribute);
        }
        else
        {
            attribute.Value = hasGiftsApplied.ToString();
            await _genericAttributeService.UpdateAttributeAsync(attribute);
        }
    }

    public async Task DeleteGiftProductMappingAsync(GiftProductMapping giftProductMapping)
    {
        await _giftProductMappingRepository.DeleteAsync(giftProductMapping);
    }

    public async Task<GiftProductMapping> GetGiftAppliedToProductAsync(int giftId, int productId)
    {
        return await _giftProductMappingRepository.Table
             .FirstOrDefaultAsync(dcm => dcm.EntityId == productId && dcm.GiftId == giftId);
    }

    public async Task<IPagedList<Product>> GetProductsWithAppliedGiftAsync(int? giftId, bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var products = _productRepository.Table.Where(product => product.HasDiscountsApplied);

        if (giftId.HasValue)
            products = from product in products
                       join dpm in _giftProductMappingRepository.Table on product.Id equals dpm.EntityId
                       where dpm.GiftId == giftId.Value
                       select product;

        if (!showHidden)
            products = products.Where(product => !product.Deleted);

        products = products.OrderBy(product => product.DisplayOrder).ThenBy(product => product.Id);

        return await products.ToPagedListAsync(pageIndex, pageSize);
    }

    public async Task InsertGiftProductMappingAsync(GiftProductMapping giftProductMapping)
    {
        await _giftProductMappingRepository.InsertAsync(giftProductMapping);
    }

}