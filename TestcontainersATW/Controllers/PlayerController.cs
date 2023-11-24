using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TestcontainersATW.DataTransfer;
using TestcontainersATW.Entities;
using TestcontainersATW.Errors;
using TestcontainersATW.Persistence;

namespace TestcontainersATW.Controllers;

[ApiController]
[Route("[controller]")]
public class PlayerController : ControllerBase
{
    private readonly AzurTechWinterContext _azurTechWinterContext;

    public PlayerController(AzurTechWinterContext azurTechWinterContext)
    {
        _azurTechWinterContext = azurTechWinterContext;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<Player>> CreatePlayer(PlayerDataTransferObject player)
    {
        var entity = (await _azurTechWinterContext.Players.AddAsync(new Player
        {
            Name = player.Name,
            HealthPoints = player.HealthPoints,
            Strength = player.Strength
        })).Entity;

        try
        {
            await _azurTechWinterContext.SaveChangesAsync();
        }
        catch (DbUpdateException e) when (e.InnerException is PostgresException postgresException)
        {
            return BadRequest(new UniqueNameViolationError
            {
                MessageText = postgresException.MessageText
            });
        }


        return CreatedAtAction(nameof(GetPlayer), new { guid = entity.Id }, entity);
    }

    [HttpGet]
    [Route("{guid}")]
    public async Task<ActionResult<Player>> GetPlayer(Guid guid)
    {
        var entity = await _azurTechWinterContext.Players.FindAsync(guid);
        if (entity is null)
        {
            return NotFound();
        }

        return Ok(entity);
    }

    [HttpGet]
    public async Task<ActionResult<Player>> GetPlayers()
    {
        return Ok(await _azurTechWinterContext.Players.ToListAsync());
    }
}