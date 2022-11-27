using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Nop.Data.Mapping;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.GiftProvider.Domain;

namespace Nop.Plugin.Misc.GiftProvider.Data.Mapping.Builders;

/// <summary>
/// Represents a discount manufacturer mapping entity builder
/// </summary>
public partial class GiftManufacturerMappingBuilder : NopEntityBuilder<GiftManufacturerMapping>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(GiftManufacturerMapping), nameof(GiftManufacturerMapping.GiftId)))
                .AsInt32().PrimaryKey().ForeignKey<Gift>()
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(GiftManufacturerMapping), nameof(GiftManufacturerMapping.EntityId)))
                .AsInt32().PrimaryKey().ForeignKey<Manufacturer>();
    }

    #endregion
}