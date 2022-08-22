using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TeamWeekAPI.Models;
using System.Linq;
using System;

namespace TeamWeekAPI.Controllers
{
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [Route("api/[controller]")]
  [ApiController]
  public class TeamsController : ControllerBase
  {
    private readonly TeamWeekAPIContext _db;

    public TeamsController(TeamWeekAPIContext db)
    {
      _db = db;
    }

    // GET api/teams
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Team>>> Get(string name, int wins, int losses)
    {
      var query = _db.Teams.AsQueryable();

      if (name != null)
      {
        query = query.Where(entry => entry.Name == name);
        query = query.Where(entry => entry.Wins == wins);
        query = query.Where(entry => entry.Losses == losses);
      }

      return await query.ToListAsync();
    }

    // POST api/teams
    [HttpPost]
    public async Task<ActionResult<Team>> Post(Team team)
    {
      _db.Teams.Add(team);
      await _db.SaveChangesAsync();

      return CreatedAtAction(nameof(GetTeam), new { id = team.TeamId }, team);
    }
    // GET: api/Teams/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Team>> GetTeam(int id)
    {
      var team = await _db.Teams.FindAsync(id);

      if (team == null)
      {
        return NotFound();
      }

      return team;
    }

    // PUT: api/Teams/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Team team)
    {
      if (id != team.TeamId)
      {
        return BadRequest();
      }

      _db.Entry(team).State = EntityState.Modified;

      try
      {
        await _db.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!TeamExists(id))
        {
          return NotFound();
        }
        else
        {
          throw;
        }
      }

      return NoContent();
    }

    // DELETE: api/Teams/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTeam(int id)
    {
      var team = await _db.Teams.FindAsync(id);
      if (team == null)
      {
        return NotFound();
      }

      _db.Teams.Remove(team);
      await _db.SaveChangesAsync();

      return NoContent();
    }

    [HttpGet("battle/{id}")]
    public async Task<IActionResult> BattleTeam(int id)
    {
      var team = await _db.Teams.FindAsync(id);
      var rand = new Random();
      int numTeams = _db.Teams.Count<Team>();
      int randTeam = 0;
      if (numTeams > 1)
      {
        randTeam = rand.Next(numTeams) + 1;
        while (randTeam == id)
        {
          randTeam = rand.Next(numTeams) + 1;
        }
        var enemyTeam = await _db.Teams.FindAsync(randTeam);
        return Battle(team, enemyTeam);
      }
      else
      {
        return NotFound();
      }
    }

    private bool TeamExists(int id)
    {
      return _db.Teams.Any(e => e.TeamId == id);
    }

    private IActionResult Battle(Team team, Team enemyTeam)
    {
      return NotFound();
    }
  }
}
