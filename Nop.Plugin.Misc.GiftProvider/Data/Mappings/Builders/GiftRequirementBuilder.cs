using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.GiftProvider.Domain;

namespace Nop.Plugin.Misc.GiftProvider.Data.Mapping.Builders;

/// <summary>
/// Represents a discount requirement entity builder
/// </summary>
public partial class GiftRequirementBuilder : NopEntityBuilder<GiftRequirement>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(GiftRequirement.GiftId)).AsInt32().ForeignKey<Gift>()
            .WithColumn(nameof(GiftRequirement.ParentId)).AsInt32().Nullable().ForeignKey<GiftRequirement>(onDelete: Rule.None);
    }

    #endregion
}