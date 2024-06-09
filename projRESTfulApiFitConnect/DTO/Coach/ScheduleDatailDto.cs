﻿namespace projRESTfulApiFitConnect.DTO.Coach
{
    public class ScheduleDatailDto
    {
        public int ClassScheduleId { get; set; }

        public string? Class { get; set; }

        public string? Coach { get; set; }

        public string? Field { get; set; }

        public DateOnly CourseDate { get; set; }

        public TimeSpan CourseTime { get; set; }

        public int MaxStudent { get; set; }

        public string? ClassStatus { get; set; }

        public decimal ClassPayment { get; set; }

        public bool CoachPayment { get; set; }
    }
}
