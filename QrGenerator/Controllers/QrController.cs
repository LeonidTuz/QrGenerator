using Microsoft.AspNetCore.Mvc;

namespace QrGenerator.Controllers;

[ApiController]
[Route("[controller]")]
public class QrController : ControllerBase
{
    [HttpGet]
    public ActionResult Get() => Ok("test");
}