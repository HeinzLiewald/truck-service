using System.ComponentModel.DataAnnotations;
using Liewald.TruckService.Application.Dtos;
using Liewald.TruckService.Application.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace Liewald.TruckService.Api.Controllers;

[Authorize]
[ApiController]
[OpenApiTags("Drivers")]
public class DriversController : ControllerBase
{
    /// <summary>
    /// Gets the list of truck drivers.
    /// </summary>
    /// <param name="location">The name of city to filter the list.</param>
    /// <returns>A list of <see cref="DriverDto"/>.</returns>
    [HttpGet]
    [Route("api/[controller]")]
    [Authorize("MustBeHeinz")]
    [OpenApiOperation("GetDrivers")]
    [ProducesResponseType(typeof(DriverDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<DriverDto>>> GetDriversAsync(
        [Required][FromQuery] string location,
        [FromServices] IDriverQueries driverQueries,
        CancellationToken cancellationToken)
    {
        IEnumerable<DriverDto> drivers =
            await driverQueries
                .GetAsync(location, cancellationToken)
                .ConfigureAwait(false);

        if (drivers.Any())
        {
            return Ok(drivers);
        }

        return NotFound();
    }

    [HttpGet]
    [Route("api/dummy")]
    [Authorize("MustBeJohn")]
    public ActionResult GetDummy()
        => Ok("A valid response here!");
}
