﻿using System;

namespace projRESTfulApiFitConnect.DTO.Coach
{
    public class CoachDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? EMail { get; set; }
        public string Photo { get; set; } = null!;
        public string? Intro { get; set; }
        public DateTime Birthday { get; set; }
        public List<ExpertiseDto>? Experties {  get; set; }
        public string Address { get; set; } = null!;
        public string? RoleDescription { get; set; } = null!;
        public string GenderDescription { get; set; } = null!;


    }
}
