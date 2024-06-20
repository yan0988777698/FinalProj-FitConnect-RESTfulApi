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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSchedule(int id)
        {
            var schdule = await _context.TclassSchedules
                            .Include(x => x.FieldReserved).Include(x => x.Class)
                            .Include(x => x.Field.Gym)
                            .FirstOrDefaultAsync(x => x.FieldReservedId == id);
            ScheduleDetailDto scheduleDetailDto = new ScheduleDetailDto
            {
                scheduleId = schdule.ClassScheduleId,
                coachId = schdule.CoachId,
                courseId = schdule.ClassId,
                maxStudent = schdule.MaxStudent,
                date = schdule.CourseDate,
                startTimeId = schdule.CourseStartTimeId,
                endTimeId = schdule.CourseEndTimeId,
                fieldId = schdule.FieldId,
                fieldName = schdule.Field.FieldName,
                gymId = schdule.Field.GymId,
                gymName = schdule.Field.Gym.GymName,
                payment = schdule.ClassPayment
            };

            return Ok(new { success = "success", scheduleDetailDto });
        }
        //修改課程為待審核
        [HttpPut]
        public async Task<IActionResult> UpdateSchedule([FromBody] string[] paymentInfo)
        {
            try
            {
                foreach (var item in paymentInfo)
                {
                    var schedule = _context.TclassSchedules.FirstOrDefault(x => x.FieldReservedId == Convert.ToInt32(item));
                    schedule.ClassStatusId = 4;
                    var field = _context.TfieldReserves.FirstOrDefault(x => x.FieldReserveId == Convert.ToInt32(item));
                    field.PaymentStatus = true;
                }
                await _context.SaveChangesAsync();
                return Ok(new { success = "success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
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
        //修改課程與場地
        [HttpPut("ReviseSchedule")]
        public async Task<IActionResult> ReviseSchedule([FromForm] ScheduleDto scheduleDto)
        {
            //TODO:完成場地及課程資料修改
            return Ok(new { success = "success" });
        }

    }
}
