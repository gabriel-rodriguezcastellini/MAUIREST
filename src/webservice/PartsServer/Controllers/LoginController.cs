using Microsoft.AspNetCore.Mvc;
using PartsServer.Models;

namespace PartsServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoginController : BaseController
{
    [HttpGet]
    public ActionResult Get()
    {
        try
        {
            string authorizationToken = Guid.NewGuid().ToString();

            PartsFactory.Initialize(authorizationToken);

            return new JsonResult(authorizationToken);
        }
        catch (Exception ex)
        {
            return new JsonResult(ex.Message);
        }
    }
}
