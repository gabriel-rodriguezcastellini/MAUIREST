using Microsoft.AspNetCore.Mvc;
using PartsServer.Models;
using System.Net;
using System.Text.Json;

namespace PartsServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PartsController : BaseController
{
    [HttpGet]
    public ActionResult Get()
    {
        bool authorized = CheckAuthorization();
        if (!authorized)
        {
            return Unauthorized();
        }
        Console.WriteLine("GET /api/parts");
        return new JsonResult(UserParts);
    }

    [HttpGet("{partid}")]
    [ProducesResponseType<Part>(StatusCodes.Status200OK)]
    public ActionResult Get(string partid)
    {
        bool authorized = CheckAuthorization();
        if (!authorized)
        {
            return Unauthorized();
        }

        if (string.IsNullOrEmpty(partid))
        {
            return BadRequest();
        }

        partid = partid.ToUpperInvariant();
        Console.WriteLine($"GET /api/parts/{partid}");
        List<Part>? userParts = UserParts;
        Part? part = userParts?.SingleOrDefault(x => x.PartID == partid);

        return part == null ? NotFound() : Ok(part);
    }

    [HttpPut("{partid}")]
    public HttpResponseMessage Put(string partid, [FromBody] Part part)
    {
        try
        {
            bool authorized = CheckAuthorization();
            if (!authorized)
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            if (!ModelState.IsValid)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            if (string.IsNullOrEmpty(part.PartID))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            Console.WriteLine($"PUT /api/parts/{partid}");
            Console.WriteLine(JsonSerializer.Serialize(part));


            List<Part>? userParts = UserParts;
            Part? existingParts = userParts?.SingleOrDefault(x => x.PartID == partid);
            if (existingParts != null)
            {
                existingParts.Suppliers = part.Suppliers;
                existingParts.PartType = part.PartType;
                existingParts.PartAvailableDate = part.PartAvailableDate;
                existingParts.PartName = part.PartName;
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
        catch (Exception)
        {
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }
    }

    [HttpPost]
    [ProducesResponseType<Part>(StatusCodes.Status200OK)]
    public ActionResult Post([FromBody] Part part)
    {
        try
        {
            bool authorized = CheckAuthorization();
            if (!authorized)
            {
                return Unauthorized();
            }

            if (!string.IsNullOrWhiteSpace(part.PartID))
            {
                return BadRequest();
            }
            Console.WriteLine($"POST /api/parts");
            Console.WriteLine(JsonSerializer.Serialize(part));

            part.PartID = PartsFactory.CreatePartID();

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            List<Part>? userParts = UserParts;

            if (userParts!.Exists(x => x.PartID == part.PartID))
            {
                return Conflict();
            }

            userParts.Add(part);

            return Ok(part);
        }
        catch (Exception)
        {
            return Problem("Internal server error");
        }
    }

    [HttpDelete]
    [Route("{partid}")]
    public HttpResponseMessage Delete(string partid)
    {
        try
        {
            bool authorized = CheckAuthorization();
            if (!authorized)
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            List<Part>? userParts = UserParts;
            Part? existingParts = userParts?.SingleOrDefault(x => x.PartID == partid);

            if (existingParts == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            Console.WriteLine($"POST /api/parts/{partid}");
            _ = userParts!.RemoveAll(x => x.PartID == partid);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
        catch (Exception)
        {
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }
    }
}
