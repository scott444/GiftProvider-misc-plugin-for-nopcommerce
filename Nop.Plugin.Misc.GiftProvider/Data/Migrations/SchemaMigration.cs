using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.GiftProvider.Domain;

namespace Nop.Plugin.Misc.GiftProvider.Data.Migrations
{
    [NopMigration("2020/01/31 11:24:16:2551771", "Gift base schema", MigrationProcessType.Update)]
    public class SchemaMigration : AutoReversingMigration
    {
        /// <summary>
        /// Collect the UP migration expressions
        /// <remarks>
        /// We use an explicit table creation order instead of an automatic one
        /// due to problems creating relationships between tables
        /// </remarks>
        /// </summary>
        public override void Up()
        {
            Create.TableFor<Gift>();
            Create.TableFor<GiftCategoryMapping>();
            Create.TableFor<GiftProductMapping>();
            Create.TableFor<GiftRequirement>();
            Create.TableFor<GiftUsageHistory>();
            Create.TableFor<GiftManufacturerMapping>();
        }
    }
}
