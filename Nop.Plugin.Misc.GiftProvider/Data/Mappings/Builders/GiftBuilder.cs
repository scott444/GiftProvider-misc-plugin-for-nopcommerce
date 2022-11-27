using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.GiftProvider.Domain;

namespace Nop.Plugin.Misc.GiftProvider.Data.Mapping.Builders;

/// <summary>
/// Represents a discount entity builder
/// </summary>
public partial class GiftBuilder : NopEntityBuilder<Gift>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
           .WithColumn(nameof(Gift.Name)).AsString(200).NotNullable()
           .WithColumn(nameof(Gift.CouponCode)).AsString(100).Nullable();
    }

    #endregion
}