using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Stores;

namespace Nop.Plugin.Misc.GiftProvider.Services;

/// <summary>
/// Represents a request of discount requirement validation
/// </summary>
public partial class GiftRequirementValidationRequest
{
    /// <summary>
    /// Gets or sets the appropriate discount requirement ID (identifier)
    /// </summary>
    public int GiftRequirementId { get; set; }

    /// <summary>
    /// Gets or sets the customer
    /// </summary>
    public Customer Customer { get; set; }

    /// <summary>
    /// Gets or sets the store
    /// </summary>
    public Store Store { get; set; }
}
