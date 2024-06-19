using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projRESTfulApiFitConnect.DTO.Schdule;
using projRESTfulApiFitConnect.Models;

namespace projRESTfulApiFitConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly GymContext _context;
        private readonly IWebHostEnvironment _env;

        public ScheduleController(GymContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        //新增課程與場地
        [HttpPost]
        public async Task<IActionResult> CreateSchedule([FromForm] ScheduleDto scheduleDto)
        {
            try
            {
                TfieldReserve tfieldReserve = new TfieldReserve
                {
                    CoachId = scheduleDto.coachId,
                    FieldId = scheduleDto.fieldId,
                    FieldDate = scheduleDto.date,
                    FieldReserveStartTime = scheduleDto.startTimeId,
                    FieldReserveEndTime = scheduleDto.endTimeId,
                    PaymentStatus = false,
                    ReserveStatus = true
                };
                _context.TfieldReserves.Add(tfieldReserve);
                _context.SaveChanges();

                TclassSchedule tclassSchedule = new TclassSchedule
                {
                    ClassId = scheduleDto.courseId,
                    CoachId = scheduleDto.coachId,
                    FieldId = scheduleDto.fieldId,
                    CourseDate = scheduleDto.date,
                    CourseTimeId = scheduleDto.startTimeId,
                    CourseStartTimeId = scheduleDto.startTimeId,
                    CourseEndTimeId = scheduleDto.endTimeId,
                    MaxStudent = scheduleDto.maxStudent,
                    ClassStatusId = 5, //場地待付款
                    ClassPayment = scheduleDto.classPayment,
                    CoachPayment = false,
                    FieldReservedId = tfieldReserve.FieldReserveId
                };
                _context.TclassSchedules.Add(tclassSchedule);

                _context.SaveChanges();
                return Ok(new { success = "success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

    }
}
