using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projRESTfulApiFitConnect.DTO.Schdule;
using projRESTfulApiFitConnect.Models;

namespace projRESTfulApiFitConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchduleController : ControllerBase
    {
        private readonly GymContext _context;
        private readonly IWebHostEnvironment _env;

        public SchduleController(GymContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        //新增課程與場地
        [HttpPost]
        public async Task<IActionResult> CreateSchdule([FromForm] SchduleDto schduleDto)
        {
            
            TfieldReserve tfieldReserve = new TfieldReserve
            {
                CoachId = schduleDto.coachId,
                FieldId = schduleDto.fieldId,
                FieldDate = schduleDto.date,
                FieldReserveStartTime = schduleDto.startTimeId,
                FieldReserveEndTime = schduleDto.endTimeId,
                PaymentStatus = false,
                ReserveStatus = true
            };
            _context.TfieldReserves.Add(tfieldReserve);
            _context.SaveChanges();
            TclassSchedule tclassSchedule = new TclassSchedule
            {
                ClassId = schduleDto.courseId,
                CoachId = schduleDto.coachId,
                FieldId = schduleDto.fieldId,
                CourseDate = schduleDto.date,
                CourseTimeId = schduleDto.startTimeId,
                CourseStartTimeId = schduleDto.startTimeId,
                CourseEndTimeId = schduleDto.endTimeId,
                MaxStudent = schduleDto.maxStudent,
                ClassStatusId = 4, //審核中
                ClassPayment = schduleDto.classPayment,
                CoachPayment = false,
                FieldReservedId = tfieldReserve.FieldId
            };
            _context.TclassSchedules.Add(tclassSchedule);
            
            _context.SaveChanges();
            return Ok(new {success = "success"});
        }

    }
}
