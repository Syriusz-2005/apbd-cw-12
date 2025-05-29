using System.ComponentModel.DataAnnotations;
using apbd_cw_12.DTOS;
using apbd_cw_12.Exceptions;
using apbd_cw_12.Services;
using Microsoft.AspNetCore.Mvc;

namespace apbd_cw_12.Controllers;

[ApiController]
[Route("/api")]
public class AppApiController(IDbService dbService) : ControllerBase
{
    [HttpGet]
    [Route("trips")]
    public async Task<IActionResult> GetTrips(
        [FromQuery] int page = 0,
        [FromQuery, Range(1, 100)] int pageSize = 10
    )
    {
        try
        {
            var result = await dbService.GetTripsAsync(page, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return Problem("Server error", statusCode: 500);
        }
    }

    [HttpPost]
    [Route("trips/{idTrip}/clients")]
    public async Task<IActionResult> AssignClientToTripAsync([FromRoute] int idTrip, [FromBody] AssignClientToTripCreateDto assignClientCreateDto)
    {
        try
        {
            await dbService.AssignClientToTripAsync(assignClientCreateDto, idTrip);
            return Ok("Client assigned");
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ConflictException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return Problem("Server error", statusCode: 500);
        }
    }

    [HttpDelete]
    [Route("clients/{idClient}")]
    public async Task<IActionResult> DeleteClientAsync([FromRoute] int idClient)
    {
        try
        {
            await dbService.DeleteClientAsync(idClient);
            return Ok("Client deleted successfully");
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ConflictException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return Problem("Server error", statusCode: 500);
        }
    }
}