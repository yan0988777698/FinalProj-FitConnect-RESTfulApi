namespace projRESTfulApiFitConnect.DTO.Schdule
{
    public class ScheduleDetailDto
    {
        public int scheduleId { get; set; }
        public int coachId { get; set; }
        public int courseId { get; set; }
        public int maxStudent { get; set; }
        public DateTime date { get; set; }
        public int? startTimeId { get; set; }
        public int? endTimeId { get; set; }
        public int fieldId { get; set; }
        public string fieldName { get; set; }
        public string gymName { get; set; }
        public int gymId { get; set; }
        public decimal payment { get; set; }
    }
}
