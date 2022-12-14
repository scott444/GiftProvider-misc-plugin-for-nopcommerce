using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.GiftProvider.Domain;

namespace Nop.Plugin.Misc.GiftProvider.Data.Mapping.Builders;

/// <summary>
/// Represents a discount usage history entity builder
/// </summary>
public partial class GiftUsageHistoryBuilder : NopEntityBuilder<GiftUsageHistory>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(GiftUsageHistory.GiftId)).AsInt32().ForeignKey<Gift>()
            .WithColumn(nameof(GiftUsageHistory.OrderId)).AsInt32().ForeignKey<Order>();
    }

    #endregion
}