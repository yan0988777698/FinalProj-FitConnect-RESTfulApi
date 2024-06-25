using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projRESTfulApiFitConnect.DTO.Coach;
using projRESTfulApiFitConnect.Models;

namespace projRESTfulApiFitConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassReservedController : ControllerBase
    {
        private readonly GymContext _context;
        private readonly IWebHostEnvironment _env;

        public ClassReservedController(GymContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        // Get: api/ClassReserved/{id}
        // 依照篩選取得課程資料
        [HttpGet("{id}")]
        public async Task<ActionResult> GetClassReserved(int id)
        {
            var reservedClass = await _context.TclassReserves.Where(x => x.ClassScheduleId == id).ToListAsync();
            return Ok(new { currentNumberOfStudent = reservedClass.Count });
        }
        //修改預約的課程狀態為待已付款
        [HttpPut]
        public async Task<IActionResult> UpdateClassReserved([FromBody] int[] courseListInfo)
        {
            try
            {
                foreach (var item in courseListInfo)
                {
                    var schedule = _context.TclassReserves.FirstOrDefault(x => x.ReserveId == item);
                    schedule.PaymentStatus = true;
                }
                await _context.SaveChangesAsync();
                return Ok(new { success = "success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

    }
}
