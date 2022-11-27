using System;
using System.Collections.Generic;
using Nop.Core.Domain.Security;
using Nop.Services.Security;

namespace Nop.Plugin.Misc.GiftProvider.Services;

/// <summary>
/// Standard permission provider
/// </summary>
public class GiftPermissionProvider : IPermissionProvider
{
    //admin area permissions
    public static readonly PermissionRecord ManageGifts = new() { Name = "Admin area. Manage Gifts", SystemName = "ManageGifts", Category = "Catalog" };

    public HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<PermissionRecord> GetPermissions()
    {
        throw new NotImplementedException();
    }


}
