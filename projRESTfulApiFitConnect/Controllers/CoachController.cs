﻿using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projRESTfulApiFitConnect.DTO;
using projRESTfulApiFitConnect.Models;

namespace projRESTfulApiFitConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoachController : ControllerBase
    {
        private readonly GymContext _context;
        private readonly IWebHostEnvironment _env;

        public CoachController(GymContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/Coach
        //取得所有教練資料(個人資料、自我介紹)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CoachDetailDto>>> GetCoaches()
        {
            string filepath = "";

            List<CoachDetailDto> coachDetailDtos = new List<CoachDetailDto>();

            if (_context.TIdentities == null)
            {
                return NotFound();
            }

            var coaches = await _context.TIdentities
                        .Where(x => x.RoleId == 2)
                        .Include(x => x.Gender)
                        .Include(x => x.TcoachInfoIds)
                        .Include(x => x.TcoachExperts)
                        .ThenInclude(te => te.Class)
                        .ToListAsync();

            foreach (var item in coaches)
            {
                var experts = item.TcoachExperts.Select(te => new ExpertiseDto
                {
                    ClassName = te.Class.ClassName
                }).ToList();
                string base64Image = "";
                filepath = Path.Combine(_env.ContentRootPath, "Images", "CoachImages", item.Photo);
                if (System.IO.File.Exists(filepath))
                {
                    byte[] bytes = await System.IO.File.ReadAllBytesAsync(filepath);
                    base64Image = Convert.ToBase64String(bytes);
                }
                string intro = string.Join(", ", item.TcoachInfoIds.Select(i => i.CoachIntro));

                CoachDetailDto coachDetailDto = new CoachDetailDto()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Phone = item.Phone,
                    EMail = item.EMail,
                    Photo = base64Image,
                    Intro = intro,
                    Birthday = item.Birthday,
                    Address = item.Address,
                    Experties = experts,
                    GenderDescription = item.Gender.GenderText
                };
                coachDetailDtos.Add(coachDetailDto);
            }
            return Ok(coachDetailDtos);
        }

        // GET: api/Coach/5
        //取得特定教練資料
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<TmemberRateClass>>> GetCoach(int id)
        {
            if (_context.TIdentities.Find(id) == null)
            {
                return Ok(new { status = "fail", message = $"編號{id}的教練不存在於資料庫" });
            }
            var rates = await _context.TmemberRateClasses
                        .Where(x => x.CoachId == id)
                        .ToListAsync();
            if (rates.Count == 0)
            {
                return Ok(new { status = "fail", message = $"編號{id}的教練尚無評價" });
            }
            return Ok(rates);
        }

        // PUT: api/Coach/5
        //修改教練資料
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCoach(int id, PutCoachDto putCoachDto)
        {
            if (id != putCoachDto.Id)
            {
                return BadRequest();
            }
            var coach = await _context.TIdentities.FindAsync(id);
            coach.Name = putCoachDto.Name;
            coach.Phone = putCoachDto.Phone;
            coach.Password = putCoachDto.Password;
            coach.EMail = putCoachDto.EMail;
            if (putCoachDto.Photo != null)
            {
                coach.Photo = putCoachDto.Photo.FileName;
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
        public async Task<ActionResult<PutCoachDto>> PostCoach(PutCoachDto putCoachDto)
        {
            TIdentity identity = new TIdentity();
            identity.Name = putCoachDto.Name;
            identity.Phone = putCoachDto.Phone;
            identity.Password = putCoachDto.Password;
            identity.EMail = putCoachDto.EMail;
            if (putCoachDto.Photo != null)
            {
                identity.Photo = putCoachDto.Photo.FileName;
            }
            identity.Birthday = putCoachDto.Birthday;
            identity.Address = putCoachDto.Address;
            identity.GenderId = putCoachDto.GenderId;
            identity.RoleId = putCoachDto.RoleId;
            _context.TIdentities.Add(identity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCoach", new { id = putCoachDto.Id }, putCoachDto);
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
