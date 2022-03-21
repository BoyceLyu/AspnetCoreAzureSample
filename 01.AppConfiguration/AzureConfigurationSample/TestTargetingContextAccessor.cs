using Microsoft.FeatureManagement.FeatureFilters;

namespace AzureConfigurationSample;

public class TestTargetingContextAccessor: ITargetingContextAccessor
{
    private const string TargetingContextLookup = "TestTargetingContextAccessor.TargetingContext";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TestTargetingContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public ValueTask<TargetingContext> GetContextAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext.Items.TryGetValue(TargetingContextLookup, out object value))
        {
            return new ValueTask<TargetingContext>((TargetingContext)value);
        }
        var groups = new List<string>();
        if (httpContext.User.Identity.Name != null)
        {
            groups.Add(httpContext.User.Identity.Name.Split("@", StringSplitOptions.None)[1]);
        }
        var targetingContext = new TargetingContext
        {
            UserId = httpContext.User.Identity.Name,
            Groups = groups
        };
        httpContext.Items[TargetingContextLookup] = targetingContext;
        return new ValueTask<TargetingContext>(targetingContext);
    }
}
