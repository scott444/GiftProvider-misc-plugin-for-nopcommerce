using FluentMigrator;
using LinqToDB.DataProvider;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Data;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.GiftProvider.Domain;
using Nop.Plugin.Misc.GiftProvider.Services;
using System;
using System.Linq;

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

    public class PermissionsMigration: AutoReversingMigration
    {
        private readonly INopDataProvider _dataProvider;

        public PermissionsMigration(INopDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public override void Up()
        {
            //GiftPermissionProvider.ManageGifts
            if (!_dataProvider.GetTable<PermissionRecord>().Any(pr => string.Compare(pr.SystemName, "ManageGifts", StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                var multiFactorAuthenticationPermission = _dataProvider.InsertEntity(
                    new PermissionRecord
                    {
                        Name = "Admin area. Manage Gifts",
                        SystemName = "ManageGifts",
                        Category = "Configuration"
                    }
                );

                //add it to the Admin role by default
                var adminRole = _dataProvider
                    .GetTable<CustomerRole>()
                    .FirstOrDefault(x => x.IsSystemRole && x.SystemName == NopCustomerDefaults.AdministratorsRoleName);

                _dataProvider.InsertEntity(
                    new PermissionRecordCustomerRoleMapping
                    {
                        CustomerRoleId = adminRole.Id,
                        PermissionRecordId = multiFactorAuthenticationPermission.Id
                    }
                );
            }
        }
    }
}
