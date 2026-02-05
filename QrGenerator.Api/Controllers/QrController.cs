using Microsoft.AspNetCore.Mvc;

namespace QrGenerator.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QrController : ControllerBase
{
    [HttpGet]
    public ActionResult Get() => Ok("test");
}