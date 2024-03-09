using Xunit.Abstractions;
using Xunit.Sdk;

namespace XUnit.Runners.Core;

public class CategoryDiscoverer : ITraitDiscoverer
{
    public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
    {
        var args = traitAttribute.GetConstructorArguments().ToList();

        if (args[0] is string[] categories)
        {
            foreach (var category in categories)
            {
                yield return new KeyValuePair<string, string>("Category", category);
            }
        }
    }
}
