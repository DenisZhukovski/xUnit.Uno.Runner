using Xunit.Sdk;

namespace XUnit.Runners.Core;

/// <summary>
/// Convenience attribute for setting a Category trait on a test or test class
/// </summary>
[TraitDiscoverer("Microsoft.Maui.CategoryDiscoverer", "Microsoft.Maui.TestUtils")]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class CategoryAttribute : Attribute, ITraitAttribute
{
    // Yes, it looks like the cateory parameter is not used; CategoryDiscoverer uses it. 
    public CategoryAttribute(params string[] categories) { }
}
