using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projRESTfulApiFitConnect.DTO.Coach;
using projRESTfulApiFitConnect.DTO.Member;
using projRESTfulApiFitConnect.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace projRESTfulApiFitConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly GymContext _context;
        private readonly IWebHostEnvironment _env;

        public MemberController(GymContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/Member
        //取得所有會員資料(個人資料)
        [HttpGet]
        public async Task<ActionResult> GetMembers()
        {
            string filepath = "";

            List<MemberDetailDto> memberDetailDtos = new List<MemberDetailDto>();

            var members = await _context.TIdentities
                        .Where(x => x.RoleId == 1)
                        .Include(x => x.Gender)
                        .Include(x => x.Role)
                        .ToListAsync();

            foreach (var item in members)
            {
                string base64Image = "";
                filepath = Path.Combine(_env.ContentRootPath, "Images", "MemberImages", item.Photo);
                if (System.IO.File.Exists(filepath))
                {
                    byte[] bytes = await System.IO.File.ReadAllBytesAsync(filepath);
                    base64Image = Convert.ToBase64String(bytes);
                }

                MemberDetailDto memberDetailDto = new MemberDetailDto()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Phone = item.Phone,
                    EMail = item.EMail,
                    Photo = base64Image,
                    Birthday = item.Birthday,
                    Address = item.Address,
                    GenderDescription = item.Gender.GenderText,
                    RoleDescription = item.Role.RoleDescribe
                };
                memberDetailDtos.Add(memberDetailDto);
            }
            return Ok(memberDetailDtos);
        }

        // GET: api/Member/5
        //取得特定會員資料(個人資料、預約課程、評論紀錄、追蹤)
        [HttpGet("{id}")]
        public async Task<ActionResult> GetMember(int id)
        {
            var member = await GetMemberByIdAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            var base64Image = await GetBase64ImageAsync(member.Photo);

            var courses = await GetDistinctClassReservesAsync(id);

            var comments = await GetCommentsAsync(id);

            var followAndBlackList = await GetFollowAndBlackList(id);

            var result = new
            {
                memberDetailDto = MapToMemberDetailDto(member, base64Image),
                reserveDetailDtos = await MapToReserveDetailDtosAsync(courses),
                comments,
                followAndBlackList
            };

            return Ok(result);
        }

        private async Task<FollowAndBlackListDto> GetFollowAndBlackList(int id)
        {
            var followAndBlackList = await _context.TmemberFollows.Where(x => x.MemberId == id).Include(x => x.Status).Include(x => x.Coach).ToListAsync();
            var follow = followAndBlackList.Where(x => x.StatusId == 1).Select(x => x.Coach.Name).ToList();
            var blackList = followAndBlackList.Where(x => x.StatusId == 2).Select(x => x.Coach.Name).ToList();
            FollowAndBlackListDto followAndBlackListDto = new FollowAndBlackListDto()
            {
                Follow = follow,
                BlackList = blackList,
            };
            return followAndBlackListDto;
        }

        private async Task<List<RateDetailDto>> GetCommentsAsync(int id)
        {
            var rates = await _context.TmemberRateClasses.Where(x => x.MemberId == id).Include(x => x.Reserve.Member).Include(x => x.Coach).ToListAsync();
            List<RateDetailDto> comments = new List<RateDetailDto>();
            foreach (var rate in rates)
            {
                RateDetailDto rateDetailDto = new RateDetailDto()
                {
                    ReserveId = rate.ReserveId,
                    Member = rate.Reserve.Member.Name,
                    Coach = rate.Coach.Name,
                    RateCoach = rate.RateCoach,
                    CoachDescribe = rate.CoachDescribe,
                    Class = rate.Reserve.ClassSchedule.Class.ClassName,
                    RateClass = rate.RateClass,
                    ClassDescribe = rate.ClassDescribe
                };
                comments.Add(rateDetailDto);
            }
            return comments;
        }

        private async Task<TIdentity> GetMemberByIdAsync(int id)
        {
            return await _context.TIdentities
                .Where(x => x.RoleId == 1 && x.Id == id)
                .Include(x => x.Gender)
                .Include(x => x.Role)
                .FirstOrDefaultAsync();
        }

        private async Task<string> GetBase64ImageAsync(string photo)
        {
            if (string.IsNullOrEmpty(photo))
            {
                return string.Empty;
            }

            var imagePath = Path.Combine(_env.ContentRootPath, @"Images\MemberImages", photo);

            if (!System.IO.File.Exists(imagePath))
            {
                return string.Empty;
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(imagePath);

            return Convert.ToBase64String(bytes);
        }

        private async Task<List<TclassReserve>> GetDistinctClassReservesAsync(int memberId)
        {
            return await _context.TclassReserves
                .Where(x => x.MemberId == memberId)
                .Include(x => x.ClassSchedule.Class)
                .Include(x => x.ClassSchedule.CourseTime)
                .Include(x => x.ClassSchedule.Coach)
                .Include(x => x.ClassSchedule.Field.Gym.Region.City)
                .GroupBy(x => new { x.ClassSchedule.ClassId, x.ClassSchedule.CoachId, x.ClassSchedule.FieldId, x.ClassSchedule.CourseDate })
                .Select(group => group.First())
                .ToListAsync();
        }

        private MemberDetailDto MapToMemberDetailDto(TIdentity member, string base64Image)
        {
            return new MemberDetailDto
            {
                Id = member.Id,
                Name = member.Name,
                Phone = member.Phone,
                EMail = member.EMail,
                Photo = base64Image,
                Birthday = member.Birthday,
                Address = member.Address,
                GenderDescription = member.Gender.GenderText,
                RoleDescription = member.Role.RoleDescribe
            };
        }

        private async Task<List<ReserveDetailDto>> MapToReserveDetailDtosAsync(List<TclassReserve> courses)
        {
            var reserveDetailDtos = new List<ReserveDetailDto>();

            foreach (var item in courses)
            {
                var sameCourseButTime = await _context.TclassSchedules
                    .Where(x => x.ClassId == item.ClassSchedule.ClassId
                                && x.CoachId == item.ClassSchedule.CoachId
                                && x.FieldId == item.ClassSchedule.FieldId
                                && x.CourseDate == item.ClassSchedule.CourseDate)
                    .Include(x => x.CourseTime)
                    .ToListAsync();

                var timeSpans = sameCourseButTime.Select(time => time.CourseTime.TimeName).ToList();

                var dateAndTimeDto = new DateAndTimeDto
                {
                    date = sameCourseButTime.FirstOrDefault()?.CourseDate,
                    timeList = timeSpans
                };

                var reserveDetailDto = new ReserveDetailDto
                {
                    ReserveId = item.ReserveId,
                    Class = item.ClassSchedule.Class.ClassName,
                    Coach = item.ClassSchedule.Coach.Name,
                    City = item.ClassSchedule.Field.Gym.Region.City.City,
                    Region = item.ClassSchedule.Field.Gym.Region.Region,
                    Gym = item.ClassSchedule.Field.Gym.Name,
                    Field = item.ClassSchedule.Field.FieldName,
                    Time = dateAndTimeDto,
                    MaxStudent = item.ClassSchedule.MaxStudent,
                    PaymentStatus = item.PaymentStatus,
                    ReserveStatus = item.ReserveStatus
                };

                reserveDetailDtos.Add(reserveDetailDto);
            }

            return reserveDetailDtos;
        }


        // PUT: api/Member/5
        //修改會員資料
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCoach(PutCoachDto putCoachDto, IFormFile img)
        {
            var coach = await _context.TIdentities.FindAsync(putCoachDto.Id);
            coach.Name = putCoachDto.Name;
            coach.Phone = putCoachDto.Phone;
            coach.Password = putCoachDto.Password;
            coach.EMail = putCoachDto.EMail;
            if (img != null)
            {
                coach.Photo = img.FileName;
            }
            coach.Birthday = putCoachDto.Birthday;
            coach.Address = putCoachDto.Address;
            coach.GenderId = putCoachDto.GenderId;
            await _context.SaveChangesAsync();

            return Ok("教練資訊已成功更新");
        }

        // POST: api/TIdentities
        //新增教練資料
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PutCoachDto>> PostCoach([FromForm] PutCoachDto putCoachDto)
        {
            TIdentity identity = new TIdentity();
            identity.Name = putCoachDto.Name;
            identity.Phone = putCoachDto.Phone;
            identity.Password = putCoachDto.Password;
            identity.EMail = putCoachDto.EMail;
            if (putCoachDto.Photo != null)
            {
                identity.Photo = putCoachDto.Photo.FileName;
                string path = Path.Combine(_env.ContentRootPath, "Images", "CoachImages", putCoachDto.Photo.FileName);
                using (FileStream fileStream = new FileStream(path, FileMode.Create))
                {
                    putCoachDto.Photo.CopyTo(fileStream);
                }
            }
            identity.Birthday = putCoachDto.Birthday;
            identity.Address = putCoachDto.Address;
            identity.GenderId = putCoachDto.GenderId;
            identity.RoleId = putCoachDto.RoleId;
            _context.TIdentities.Add(identity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCoach", new { id = identity.Id }, putCoachDto);
        }

        // DELETE: api/TIdentities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTIdentity(int id)
        {
            var tIdentity = await _context.TIdentities.FindAsync(id);
            if (tIdentity == null)
            {
                return NotFound();
            }

            var info = await _context.TcoachInfoIds.Where(x => x.CoachId == id).ToListAsync();
            _context.TcoachInfoIds.RemoveRange(info);
            var expert = await _context.TcoachExperts.Where(x => x.CoachId == id).ToListAsync();
            _context.TcoachExperts.RemoveRange(expert);
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
