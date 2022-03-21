using Microsoft.AspNetCore.Mvc;

namespace AzureConfigurationSample.Controllers;


[ApiController]
[Route("api/[controller]")]
[Route("api/v{version:apiVersion}/[controller]")]
public class TestController:ControllerBase
{
    [HttpGet("Test1")]
    [ApiVersion("1")]
    public string Test1()
    {
        return "Test1";
    }

    [HttpGet("Test1")]
    [ApiVersion("2")]
    public string TestV2()
    {
        return "Test2";
    }
}
