using Microsoft.AspNetCore.Mvc;
using PartsServer.Models;
using System.Net;

namespace PartsServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BaseController : ControllerBase
{
    protected List<Part>? UserParts
    {
        get
        {
            if (string.IsNullOrWhiteSpace(AuthorizationToken))
            {
                return null;
            }

            if (!PartsFactory.Parts.TryGetValue(AuthorizationToken, out Tuple<DateTime, List<Part>>? value))
            {
                return null;
            }

            Tuple<DateTime, List<Part>> result = value;

            return result.Item2;
        }
    }

    protected bool CheckAuthorization()
    {
        PartsFactory.ClearStaleData();
        HttpContext ctx = HttpContext;

        if (ctx != null)
        {
            if (string.IsNullOrWhiteSpace(AuthorizationToken))
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return false;
            }
        }
        else
        {
            return false;
        }

        return PartsFactory.Parts.ContainsKey(AuthorizationToken);
    }

    protected string AuthorizationToken
    {
        get
        {
            string authorizationToken = string.Empty;

            HttpContext ctx = HttpContext;
            if (ctx != null)
            {
                authorizationToken = ctx.Request.Headers.Authorization.ToString();
            }

            return authorizationToken;
        }
    }
}
