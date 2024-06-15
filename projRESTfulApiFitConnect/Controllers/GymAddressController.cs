using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projRESTfulApiFitConnect.Models;

namespace projRESTfulApiFitConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GymAddressController : ControllerBase
    {
        private readonly GymContext _db;
        private readonly IWebHostEnvironment _env;
        public GymAddressController(GymContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
        //取得場館
        [HttpGet("GetGym")]
        public async Task<IActionResult> GetGym()
        {
            var roads = await _db.TGyms.Select(x => new { x.GymId,x.GymName, x.GymAddress }).OrderBy(x=>x.GymAddress).ToListAsync();
            return Ok(roads);
        }
        [HttpGet("GetField/{id}")]
        public async Task<IActionResult> GetField(int id)
        {
            var fields = await _db.Tfields.Where(x=>x.GymId==id).Select(x => new { x.FieldId, x.Floor, x.FieldName,x.FieldPayment }).ToListAsync();
            return Ok(fields);
        }

    }
}
