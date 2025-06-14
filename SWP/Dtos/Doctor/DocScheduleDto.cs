namespace SWP.Dtos.Doctor
{
    public class DocScheduleDto
    {
        public DateOnly? Date { get; set; }
        public TimeOnly? Start { get; set; }
        public TimeOnly? End { get; set; }
        public string Room { get; set; }

    }
}
