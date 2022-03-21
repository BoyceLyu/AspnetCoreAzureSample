using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace AzureConfigurationSample.Pages
{
    public class IndexModel : PageModel
    {
        public Settings Settings { get; }

        public IndexModel(IOptionsSnapshot<Settings> options)
        {
            Settings = options.Value;
        }
    }
}