using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projRESTfulApiFitConnect.DTO.Schdule;
using projRESTfulApiFitConnect.Models;

namespace projRESTfulApiFitConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FieldController : ControllerBase
    {
        private readonly GymContext _context;
        private readonly IWebHostEnvironment _env;

        public FieldController(GymContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteField(int id)
        {
            var deletedField = _context.TfieldReserves.Find(id);
            if (deletedField == null)
                return Ok(new { success = "資料不存在" });
            _context.TfieldReserves.Remove(deletedField);
            _context.SaveChanges();
            return Ok(new { success = "成功刪除" });
        }
    }
}
