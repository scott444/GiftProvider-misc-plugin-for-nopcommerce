using System;
using System.Collections.Generic;
using Nop.Data.Mapping;
using Nop.Plugin.Misc.GiftProvider.Domain;

namespace Nop.Plugin.Misc.GiftProvider.Data.Mapping
{
    /// <summary>
    /// Base instance of backward compatibility of table naming
    /// </summary>
    public partial class BaseNameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new()
        {
            { typeof(GiftCategoryMapping), "Gift_AppliedToCategories" },
            { typeof(GiftManufacturerMapping), "Gift_AppliedToManufacturers" },
            { typeof(GiftProductMapping), "Gift_AppliedToProducts" },
        };

        public Dictionary<(Type, string), string> ColumnName => new()
        {
            { (typeof(GiftCategoryMapping), "GiftId"), "Gift_Id" },
            { (typeof(GiftCategoryMapping), "EntityId"), "Category_Id" },
            { (typeof(GiftManufacturerMapping), "GiftId"), "Gift_Id" },
            { (typeof(GiftManufacturerMapping), "EntityId"), "Manufacturer_Id" },
            { (typeof(GiftProductMapping), "GiftId"), "Gift_Id" },
            { (typeof(GiftProductMapping), "EntityId"), "Product_Id" },
        };
    }
}