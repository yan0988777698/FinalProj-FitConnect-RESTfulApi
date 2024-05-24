using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projRESTfulApiFitConnect.Models;

namespace projRESTfulApiFitConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TIdentitiesController : ControllerBase
    {
        private readonly GymContext _context;

        public TIdentitiesController(GymContext context)
        {
            _context = context;
        }

        // GET: api/TIdentities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TIdentity>>> GetTIdentities()
        {
          if (_context.TIdentities == null)
          {
              return NotFound();
          }
            return await _context.TIdentities.Where(x=>x.RoleId==2).ToListAsync();
        }

        // GET: api/TIdentities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TIdentity>> GetTIdentity(int id)
        {
          if (_context.TIdentities == null)
          {
              return NotFound();
          }
            var tIdentity = await _context.TIdentities.FindAsync(id);

            if (tIdentity == null)
            {
                return NotFound();
            }

            return tIdentity;
        }

        // PUT: api/TIdentities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTIdentity(int id, TIdentity tIdentity)
        {
            if (id != tIdentity.Id)
            {
                return BadRequest();
            }

            _context.Entry(tIdentity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TIdentityExists(id))
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

        // POST: api/TIdentities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TIdentity>> PostTIdentity(TIdentity tIdentity)
        {
          if (_context.TIdentities == null)
          {
              return Problem("Entity set 'GymContext.TIdentities'  is null.");
          }
            _context.TIdentities.Add(tIdentity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTIdentity", new { id = tIdentity.Id }, tIdentity);
        }

        // DELETE: api/TIdentities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTIdentity(int id)
        {
            if (_context.TIdentities == null)
            {
                return NotFound();
            }
            var tIdentity = await _context.TIdentities.FindAsync(id);
            if (tIdentity == null)
            {
                return NotFound();
            }

            _context.TIdentities.Remove(tIdentity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TIdentityExists(int id)
        {
            return (_context.TIdentities?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
