using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Catalog;
using Nop.Data.Extensions;
using Nop.Data.Mapping;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.GiftProvider.Domain;

namespace Nop.Plugin.Misc.GiftProvider.Data.Mapping.Builders;

/// <summary>
/// Represents a discount product mapping entity builder
/// </summary>
public partial class GiftProductMappingBuilder : NopEntityBuilder<GiftProductMapping>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(GiftProductMapping), nameof(GiftProductMapping.GiftId)))
                .AsInt32().PrimaryKey().ForeignKey<Gift>()
            .WithColumn(NameCompatibilityManager.GetColumnName(typeof(GiftProductMapping), nameof(GiftProductMapping.EntityId)))
                .AsInt32().PrimaryKey().ForeignKey<Product>();
    }

    #endregion
}